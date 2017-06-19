using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse.SearchHandling
{
    internal class HighEntropyWordWGoogleSearchHandler : HighEntropyWordSearchHandler
    {

        private readonly int _max;
        private readonly int _limit;


        public static string ApiKey { get; set; }
        public static string SearchEngineId { get; set; } = "017576662512468239146";

        public HighEntropyWordWGoogleSearchHandler(int maxSearches, int toManyResults, double lowestInterestingEntropy) : base(lowestInterestingEntropy)
        {
            _max = maxSearches;
            _limit = toManyResults;
        }
        public new string OutputName { get; } = "HighEntropyWordsWGoogle";

        public new List<GoogleEntropyResult> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            var hew = base.Process(apk, results).Cast<EntropyResult>().ToList();

            return (_max == -1 || _max > hew.Count)
                ? hew.Select(s => new GoogleEntropyResult(s)).Where(g => g.GoogleResults != 1 && g.GoogleResults < _limit + 1).ToList()
                : hew.Take(_max).Select(s => new GoogleEntropyResult(s)).Where(g => g.GoogleResults < _limit + 1).ToList();
        }

    }

    class GoogleEntropyResult : EntropyResult
    {
        public GoogleEntropyResult(EntropyResult res) : base(res.Word, res.Entropy, res.File, res.Line, res.Index)
        {
            GoogleResults = GoogleCustomSearch(res.Word);
        }

        private static readonly Regex ResCountRegex = new Regex("\"totalResults\": \"([0-9]+)\"", RegexOptions.Compiled);
        private static WebClient _wc = new WebClient();
        private static long GoogleCustomSearch(string word)
        {
            var query =
                $"https://www.googleapis.com/customsearch/v1?" +
                $"key={HighEntropyWordWGoogleSearchHandler.ApiKey}&" +
                $"cx={HighEntropyWordWGoogleSearchHandler.SearchEngineId}:omuauf_lfve&" +
                $"q={WebUtility.UrlEncode(word)}";
            try
            {
                var html = _wc.DownloadString(query);
                var m = ResCountRegex.Match(html);
                if (m.Success)
                    return Int64.Parse(m.Groups[1].Value.Replace(",", ""));
                return -1;
            }
            catch (WebException)
            {
                return -1;
            }
        }

        public long GoogleResults { get; set; }
    }
}