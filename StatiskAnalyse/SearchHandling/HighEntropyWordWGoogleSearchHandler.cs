using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse.SearchHandling
{
    internal class HighEntropyWordWGoogleSearchHandler : HighEntropyWordSearchHandler
    {
        private readonly int _max;
        private readonly int _limit;

        public HighEntropyWordWGoogleSearchHandler(int maxSearches, int toManyResults, double lowestInterestingEntropy) : base(lowestInterestingEntropy)
        {
            _max = maxSearches;
            _limit = toManyResults;
        }
        public string OutputName { get; } = "HighEntropyWordsWGoogle";

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            var hew = base.Process(apk, results).Cast<EntropyResult>();

            return _max == -1
                ? hew.Select(s => new GoogleSearch(s.Word)).Where(g => g.Results != 1 && g.Results < _limit + 1).Cast<object>().ToList()
                : hew.Take(_max).Select(s => new GoogleSearch(s.Word))
                    .Where(g => g.Results < _limit + 1).Cast<object>().ToList();
        }
    }
}