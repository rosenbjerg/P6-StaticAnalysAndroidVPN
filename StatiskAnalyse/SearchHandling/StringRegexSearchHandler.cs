using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class StringRegexSearchHandler : IRegexSearchHandler
    {
        public StringRegexSearchHandler(string outputName, IEnumerable<string> strings) 
        {
            OutputName = outputName;
            Regex = new Regex(string.Join("|", strings.Select(s => "(" + s + ")")), RegexOptions.Compiled);
        }
    
        public string OutputName { get; }

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results.Cast<object>().ToList();
        }

        public Regex Regex { get; }
    }
}