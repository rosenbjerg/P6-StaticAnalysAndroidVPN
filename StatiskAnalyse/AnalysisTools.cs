using System.Collections.Generic;
using System.Linq;
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
                end = end + 1;
                prev = file.Source[end];
            }
            return end - 1;
        }

        private static int GetLineWith(string find, ClassFile file, int line, int start)
        {
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

        public static string GetStringFromRegister(ClassFile file, string register, int line)
        {
            int i = line;
            var l = file.Source[i];
            var end1 = "move-result-object " + register;
            var end2 = "const-string " + register + ", ";
            var end3 = "const-string/jumbo " + register + ", ";
            while (!l.EndsWith(end1) && !l.Contains(end2) && !l.Contains(end3))
                l = file.Source[--i];
            if (l.EndsWith(end1))
            { // String is from a stringbuilder
                while (!l.Contains("invoke-virtual {"+register+"}"))
                    l = file.Source[--i];
                return TraceStringBuilder(file, i, register);
            }
            if (l.Contains(end2) || l.Contains(end3))
                return Util.ConstantStringRegex.Match(l).Groups[3].Value;
            return "";
        }

        public static string[] TraceArray(ClassFile file, int line, string register)
        {
            if (register.StartsWith("p"))
                return new[] {"\\" + register};
            int i = line;
            var l = file.Source[--i];
            var end1 = "new-array " + register;
            while (!l.Contains(end1))
                l = file.Source[--i];
            var m = Util.NewArrayRegex.Match(l);
            var rm = GetRegisters(file, i);
            var s = rm.Get(m.Groups[2].Value);
            var arr = new string[ParseInt(s)];
            for (int j = i+1; j < line; j++)
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

        private static int ParseInt(string input)
        {
            if (input.StartsWith("0x"))
                return int.Parse(input.Substring(2), NumberStyles.HexNumber);
            return int.Parse(input);
        }

        public static string TraceStringBuilder(ClassFile file, int line, string register)
        {
            var startLine = GetMethodStart(file, line);
            var endLine = GetMethodEnd(file, line);
            var ni = GetLineWith($"new-instance {register}, Ljava/lang/StringBuilder", file, line, startLine);
            var rm = new RegisterMachineSimulator();
            var sb = new StringBuilder();
            for (int i = ni; i < endLine; i++)
            {
                var tl = file.Source[i];
                if (tl == "") continue;
                if (tl.EndsWith("Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;"))
                {
                    var m = Util.InvokeVirtualRegex.Match(tl);
                    sb.Append(rm.Get(m.Groups[2].Value));
                }
                if (tl.Contains("invoke-virtual {"+register+"}, Ljava/lang/StringBuilder;->toString()Ljava/lang/String;"))
                     return sb.ToString();
                rm.ParseLine(tl);

            }

            return sb.ToString();
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