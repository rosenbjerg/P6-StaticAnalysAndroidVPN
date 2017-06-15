using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class StringSearchHandler : IConstantStringSearchHandler
    {
        private readonly List<string> _strings;

        public StringSearchHandler(string outputName, IEnumerable<string> strings)
        {
            OutputName = outputName;
            _strings = strings.ToList();
        }

        public string OutputName { get; }

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results
                .Where(
                    x => _strings.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " ")))
                .Cast<object>().ToList();
        }
    }
}