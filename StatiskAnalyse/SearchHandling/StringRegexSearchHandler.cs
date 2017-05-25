using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class StringRegexSearchHandler : RegexSearchHandler
    {
        public StringRegexSearchHandler(string outputName, IEnumerable<string> strings) : base(new Regex(string.Join("|",strings.Select(s => "("+s+")"))))
        {
            OutputName = outputName;
        }

        public override string OutputName { get; }

        public override List<object> Process(IEnumerable<Use> results)
        {
            return results.Cast<object>().ToList();
        }
    }
}