using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse
{
    public static class AnalysisTools
    {

        public static void TraceStringBuilder(string a)
        {
            
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