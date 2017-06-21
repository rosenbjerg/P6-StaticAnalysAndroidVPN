using System.Collections.Generic;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using System.Globalization;
using System.Text;


namespace StatiskAnalyse
{
    public static class AnalysisTools
    {
        /// <summary>
        /// Finds the index of the first line of the method containing the input line
        /// </summary>
        private static int GetMethodStart(ClassFile file, int line)
        {
            var start = line;
            var prev = file.Source[start];
            while (!prev.StartsWith(".method"))
            {
                if (--start < 0) return -1;
                prev = file.Source[start];
            }
            return start + 2;
        }

        /// <summary>
        /// Finds the index of the last line of the method containing the input line
        /// </summary>
        private static int GetMethodEnd(ClassFile file, int line)
        {
            var prev = "";
            var end = line;
            while (!prev.StartsWith(".end method"))
            {
                prev = file.Source[++end];
            }
            return end - 1;
        }

        private static int GetLineWith(string find, ClassFile file, int line, int start = -1)
        {
            if (start == -1)
                start = GetMethodStart(file, line);
            for (int i = line; i > start; i--)
            {
                if (file.Source[i].Contains(find))
                    return i;
            }
            return -1;
        }

        public static string GetMethodInputTypes(ClassFile file, int line)
        {
            var start = GetMethodStart(file, line) - 2;
            if (start < 0)
                return "";
            var l = file.Source[start];
            var m = Util.MethodRegex.Match(l);
            if (!m.Success)
                return "";
            return m.Groups[5].Value;
        }

        public static string GetMethodName(ClassFile file, int line)
        {
            var start = GetMethodStart(file, line) - 2;
            if (start < 0)
                return "";
            var l = file.Source[start];
            var m = Util.MethodRegex.Match(l);
            if (!m.Success)
                return "";
            return m.Groups[4].Value;
        }

        public static string GetClassName(ClassFile file)
        {
            var m = Util.ClassRegex.Match(file.Source[0]);
            return m.Groups[4].Value.Substring(1);
        }

        public static RegisterMachineSimulator GetRegisters(ClassFile file, int line)
        {
            var start = GetMethodStart(file, line);
            var rm = new RegisterMachineSimulator();
            for (int i = start; i < line; i++)
            {
                rm.ParseLine(file.Source[i]);
            }
            return rm;
        }

        private static readonly Regex[] _registerAssignments = new[]
        {
            new Regex(" *const[^ ]* (v\\d+).*", RegexOptions.Compiled),
            new Regex(" *move-result[^ ]* (v\\d+).*", RegexOptions.Compiled),
            new Regex(" *iget[^ ]* (v\\d+).*", RegexOptions.Compiled),
            new Regex(" *aget[^ ]* (v\\d+).*", RegexOptions.Compiled),
            new Regex(" *sget[^ ]* (v\\d+).*", RegexOptions.Compiled),
            //new Regex(" *invoke-virtual {(v\\d+)}, Ljava/lang/StringBuilder;->toString.*", RegexOptions.Compiled),

        };
        private static Regex _ignoreable = new Regex(" *(([ias]get)|const|move-r|(invoke-virtual.*Ljava/lang/StringBuilder;->toString.*)).*", RegexOptions.Compiled);
        private static Regex _sbTs = new Regex(" *invoke-virtual {(v\\d+)}, Ljava/lang/StringBuilder;->toString.*", RegexOptions.Compiled);
            
        public static string GetRegisterValue(ClassFile file, string register, int line)
        {
            var start = GetMethodStart(file, line);
            string l = "";
            int i;
            Match match = null;
            for (i = line; i > start; i--)
            {
                l = file.Source[i];
                if (!_ignoreable.Match(l).Success)
                    continue;
                foreach (var rx in _registerAssignments)
                {
                    var m = rx.Match(l);
                    if (!m.Success || m.Groups[1].Value != register)
                        continue;
                    match = m;
                    break;
                }
                if (match != null)
                    break;
            }

            if (match != null && match.Success)
            {
                var v = match.Value;
                    
                if (v.Contains("move-result"))
                {
                    var m = Util.InvokeRegex.Match(file.Source[i-2]);
                    var type = m.Groups[4].Value;
                    var method = m.Groups[5].Value;
                    if ((type == "Ljava/lang/StringBuilder" || type == "Ljava/lang/StringBuffer") && method == "toString")
                    {
                        return TraceStringBuilder(file, i, register);
                    }
                }
                if (v.Contains("const"))
                {
                    return Util.ConstantStringRegex.Match(l).Groups[3].Value;
                }
                if (v.Contains("iget"))
                {
                    var m = Util.IGetRegex.Match(v);
                    return $"({m.Groups[4].Value}->{m.Groups[5].Value})";
                }
                if (v.Contains("aget"))
                {
                    var m = Util.AGetRegex.Match(v);
                    return $"({m.Groups[3].Value}[{GetRegisterValue(file, m.Groups[4].Value, i)}])";
                }
                if (v.Contains("sget"))
                {
                    var m = Util.SGetRegex.Match(v);
                    return $"({m.Groups[2].Value}->{m.Groups[3].Value})";
                }
            }
            return "";
        }

        public static string[] TraceArray(ClassFile file, int line, string register)
        {
            if (register.StartsWith("p"))
                return new[] {"?(" + register + ")"};
            int i = GetLineWith("new-array " + register, file, line);
            int i2 = GetLineWith("move-result-object " + register, file, line);
            if (i > i2)
            {
                var l = file.Source[i];
                var m = Util.NewArrayRegex.Match(l);
                var rm = GetRegisters(file, i);
                var s = rm.Get(m.Groups[2].Value);
                var arr = new string[ParseInt(s)];
                for (int j = i + 1; j < line; j++)
                {
                    l = file.Source[j];
                    if (l == "") continue;
                    rm.ParseLine(l);
                    m = Util.APutRegex.Match(l);
                    if (!m.Success || m.Groups[3].Value != register) continue;
                    var index = ParseInt(rm.Get(m.Groups[4].Value));
                    arr[index] = rm.Get(m.Groups[2].Value);
                }
                return arr;
            }
            var l2 = file.Source[i2-2];
            var m2 = Util.InvokeRegex.Match(l2);
            if (m2.Groups[5].Value != "toArray")
                return null;
            return TraceList(file, i2 - 2, GetParameter(m2.Groups[3].Value, 0)).ToArray();
        }

        private static string GetParameter(string group, int i)
        {
            return group.Split(',')[i].Trim();
        }

        private static int ParseInt(string input)
        {
            if (input.StartsWith("0x"))
                return int.Parse(input.Substring(2), NumberStyles.HexNumber);
            int ret;
            if (int.TryParse(input, out ret))
                return ret;
            else
            {
                return -1;
            }
        }

        public static string TraceStringBuilder(ClassFile file, int line, string register)
        {
            var startLine = GetMethodStart(file, line);
            var ni = GetLineWith($"new-instance {register}, Ljava/lang/StringBu", file, line, startLine);
            var rm = new RegisterMachineSimulator();
            var sb = new StringBuilder();
            for (int i = ni; i < line; i++)
            {
                var tl = file.Source[i];
                if (tl == "") continue;
                if (tl.EndsWith("er;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;"))
                {
                    var m = Util.InvokeRegex.Match(tl);
                    var reg = m.Groups[3].Value.Split(',')[1].Trim();
                    sb.Append(rm.Get(reg));
                }
                if (tl.Contains("{" + register + "}") && tl.EndsWith("er;->toString()Ljava/lang/String;"))
                     return sb.ToString();
                rm.ParseLine(tl);

            }

            return sb.ToString();
        }

        public static List<string> TraceList(ClassFile file, int line, string register)
        {
            var startLine = GetMethodStart(file, line);
            var ni = GetLineWith($"new-instance {register}, Ljava/util/ArrayList", file, line, startLine);
            var rm = new RegisterMachineSimulator();
            var list = new List<string>();
            for (int i = ni; i < line; i++)
            {
                var tl = file.Source[i];
                if (tl == "") continue;
                if (tl.EndsWith("Ljava/util/List;->add(Ljava/lang/Object;)Z"))
                {
                    var m = Util.InvokeRegex.Match(tl);
                    var reg = m.Groups[3].Value.Split(',')[1].Trim();
                    list.Add(rm.Get(reg));
                }
                if (tl.Contains(register + "}, Ljava/util/List;->toArray([Ljava/lang/Object;)[Ljava/lang/Object;"))
                    return list;
                rm.ParseLine(tl);

            }

            return list;
        }

        public static IEnumerable<Use> TraceMethodCall(ApkAnalysis a, Use result)
        {
            string method = GetMethodName(result.FoundIn, result.Line);
            string type = GetMethodInputTypes(result.FoundIn, result.Line);
            var cl = GetClassName(result.FoundIn);
            var s = " *invoke.*L" + Regex.Escape(cl) + ";->" + Regex.Escape(method) + "\\("+Regex.Escape(type) +"\\).*";
            var uses = a.Root.FindUses(new Regex(s, RegexOptions.Compiled));
            return uses;
        }
    }
}