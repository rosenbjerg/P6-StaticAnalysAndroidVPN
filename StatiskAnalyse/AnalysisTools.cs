using System;
<<<<<<< HEAD
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using StatiskAnalyse.ResultWrappers;
=======
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
>>>>>>> b770f2c898576799ecd69aa09bedbdef3c47de8c

namespace StatiskAnalyse
{
    public static class AnalysisTools
    {
        /// <summary>
        /// Finds the index of the first line of the method containing the input line
        /// </summary>
        private static int GetMethodStart(ClassFile file, int line)
        {
            var prev = "";
            var start = line;
            while (!prev.StartsWith(".method"))
            {
                start = start - 1;
                prev = file.Source[start];
            }
            return start + 1;
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

<<<<<<< HEAD
        public static void TraceStringBuilder(string a)
=======

        private static int GetLineWith(string find, ClassFile file, int line, int start)
        {
            for (int i = line; i > start; i--)
            {
                if (file.Source[i].Contains(find))
                    return i;
            }
            return -1;
        }

        internal static string TraceStringBuilder(ApkAnalysis apk, ClassFile file, int line, string register)
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
    }

    internal static class Util
    {
        public static Regex ConstantStringRegex = new Regex($" *const-string (v[0-9]+), \"(.+)\"", RegexOptions.Compiled);
        public static Regex NewInstanceRegex = new Regex(" *new-instance (v[0-9]+), ([\\w/]+);", RegexOptions.Compiled);
        public static Regex MoveResultObjectRegex = new Regex(" *move-result-object (v[0-9]+)", RegexOptions.Compiled);
        public static Regex InvokeVirtualRegex = new Regex(" *invoke-virtual {(v\\d+), ([vp]\\d+)}, ([\\w\\/]+);->\\w+\\(([\\w\\/;]+)\\)([\\w\\/]+);", RegexOptions.Compiled);
        public static Regex InvokeDirectRegex = new Regex(" *invoke-direct {(v\\d+)?}, ([\\w\\/]+);->(<?[\\w]+>?)\\(([\\w\\/;]+)\\)([\\w\\/]+)", RegexOptions.Compiled);
        public static Regex InvokeStaticRegex = new Regex(" *invoke-static {(v\\d+)?}, ([\\w\\/]+);->\\w+\\(([\\w\\/;]*)\\)([\\w\\/]+);", RegexOptions.Compiled);
        public static Regex MethodRegex = new Regex("\\.method ([a-z]+) (static)? ?(\\w+)\\(([\\w\\/;]+)\\)([\\w\\/]+)", RegexOptions.Compiled);
    }

    class RegisterMachineSimulator
    {
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _dictType = new Dictionary<string, string>();
        private string _latestResult = "";
        private string _latestResultType = "";

        public string Get(string register)
        {
            if (register.StartsWith("p"))
                return "\\" + register;
            string ret;
            return _dict.TryGetValue(register, out ret) ? ret : "";
        }
        public string GetType(string register)
        {
            string ret;
            return _dictType.TryGetValue(register, out ret) ? ret : "";
        }

        public void ParseLine(string line)
        {
            Match m;
            m = Util.ConstantStringRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, m.Groups[2].Value, "Ljava/lang/String");
            m = Util.NewInstanceRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, "new-instance", m.Groups[2].Value);
            m = Util.MoveResultObjectRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, _latestResult, _latestResultType);
            m = Util.InvokeVirtualRegex.Match(line);
            if (m.Success)
            {
                var t = m.Groups[5].Value;
                _latestResult = "Object";
                _latestResultType = t;
            }
            m = Util.InvokeStaticRegex.Match(line);
            if (m.Success)
            {
                var t = m.Groups[4].Value;
                _latestResult = "Object";
                _latestResultType = t;
            }

            //m = Util.InvokeVirtualRegex.Match(line);
            //if (m.Success)
            //{
            //    var v = m.Groups[3].Value;
            //}

        }

        private void UpdateRegister(string reg, string val, string type)
>>>>>>> b770f2c898576799ecd69aa09bedbdef3c47de8c
        {
            _dict[reg] = val;
            _dictType[reg] = type;
        }

        public static void TraceMethodCall(ApkAnalysis a, Use result, int i)
        {
            string function = result.FoundIn.Source[i].Split( ).Last();
            Tuple<int, string>[] calledlocations;
            string pattern = @"invoke-\w*";
            Console.WriteLine(a.Root.DirPath);
            var uses = a.Root.FindUses(new Regex(pattern));




            return;
        }
    }
}