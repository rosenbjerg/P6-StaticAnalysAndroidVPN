using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using StatiskAnalyse.ResultWrappers;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


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





        private static int GetLineWith(string find, ClassFile file, int line, int start)
        {
            for (int i = line; i > start; i--)
            {
                if (file.Source[i].Contains(find))
                    return i;
            }
            return -1;
        }

        public static string GetMethodName(ClassFile file, int line)
        {
            var start = GetMethodStart(file, line) -1;
            var m = Util.MethodRegex.Match(file.Source[start]);
            return m.Groups[3].Value;
        }

        public static string GetClassName(ClassFile file)
        {
            var m = Util.ClassRegex.Match(file.Source[0]);
            return m.Groups[2].Value;
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

        public static void TraceMethodCall(ApkAnalysis a, Use result, int i)
        {
            string function = GetMethodName(result.FoundIn, i);
            var cl = GetClassName(result.FoundIn).Replace("/", "\\/");
            var s = " *invoke.*" + cl + ";->" + function + ".*"; 
            var uses = a.Root.FindUses(new Regex(s, RegexOptions.Compiled), 0);
            foreach (var use in uses)
            {
              //  Console.WriteLine(use.SampleLine);
            }




        }
    }

    internal static class Util
    {
        private const string _reg = "(v\\d+)";
        private const string _regOrParam = "([vp]\\d+)";
        private const string _type = "([^ :;]+)";
        private const string _fieldOrMethod = "([^>:\\(]+)";
        private const string _inputTypes = "([^ :]+)";


        public static Regex ConstantStringRegex =
            new Regex($" *const-string " + _reg + ", \"(.+)\"", 
                RegexOptions.Compiled);

        public static Regex NewInstanceRegex =
            new Regex(" *new-instance " + _reg + ", " + _type + ";", 
                RegexOptions.Compiled);

        public static Regex MoveResultObjectRegex = 
            new Regex(" *move-result-object " + _reg,
                RegexOptions.Compiled);

        public static Regex InvokeVirtualRegex =
            new Regex(
                " *invoke-virtual {" + _reg + ", " + _regOrParam + "}, " + _type + ";->" + _fieldOrMethod + "\\(" + _inputTypes + "\\)" + _type + ";", 
                RegexOptions.Compiled);

        public static Regex InvokeDirectRegex =
            new Regex(
                " *invoke-direct {" + _reg + "?}, " + _type + ";->" + _fieldOrMethod + "\\(" + _inputTypes + "\\)" + _type, 
                RegexOptions.Compiled);

        public static Regex InvokeStaticRegex =
            new Regex(
                " *invoke-static {" + _reg + "?}, " + _type + ";->" + _fieldOrMethod + "\\(" + _inputTypes + "\\)" + _type + ";", 
                RegexOptions.Compiled);

        public static Regex ClassRegex = 
            new Regex("\\.class ([a-z]+)? ?" + _type + ";", 
                RegexOptions.Compiled);

        public static Regex MethodRegex =
            new Regex("\\.method ([a-z]+) ?(static)? " + _fieldOrMethod + "\\(([\\w\\/;]+)\\)([\\w\\/]+)",
                RegexOptions.Compiled);

        public static Regex IGetRegex =
            new Regex(" *iget-object " + _reg + ", " + _regOrParam + ", " + _type + ";->" + _fieldOrMethod + ":" + _type + ";", 
                RegexOptions.Compiled);
    }
    
}