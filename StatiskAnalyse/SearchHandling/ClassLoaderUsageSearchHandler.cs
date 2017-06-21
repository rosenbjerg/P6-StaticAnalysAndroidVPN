using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    class ClassLoaderUsageSearchHandler : IRegexSearchHandler
    {
        public string OutputName { get; } = "ClassLoaderUsage";

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results.Cast<object>().ToList();
        }

        public Regex Regex { get; } = new Regex(" *invoke-.* ([^ ]+ClassLoader);->defineClass.*", RegexOptions.Compiled);
    }
}