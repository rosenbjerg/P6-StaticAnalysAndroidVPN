using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class reflectionSearchHandler : IRegexSearchHandler
    {
        public string OutputName { get; protected set; } = "Reflection";

        public virtual List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results.Cast<object>().ToList();
        }
        
        public Regex Regex { get; protected set; } = new Regex(".*Ljava/lang/reflect.*", RegexOptions.Compiled);

    }
} 