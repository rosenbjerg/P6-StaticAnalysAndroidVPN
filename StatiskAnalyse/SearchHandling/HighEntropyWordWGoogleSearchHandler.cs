using System.Collections.Generic;
using System.Linq;

namespace StatiskAnalyse
{
    class HighEntropyWordWGoogleSearchHandler : HighEntropyWordSearchHandler
    {
        public override string OutputName { get; } = "HighEntropyWordsWGoogle";
        public override List<object> Process(IEnumerable<Use> results)
        {
            var hew = base.Process(results).Cast<EntropyResult>();

            return ApkAnalysis.MaxSearchesPerApp == -1
                ? hew.Select(s => new GoogleSearch(s.Word)).Where(g => g.Results != -1).Cast<object>().ToList()
                : hew.Take(ApkAnalysis.MaxSearchesPerApp).Select(s => new GoogleSearch(s.Word)).Where(g => g.Results != -1).Cast<object>().ToList();
        }
    }
}