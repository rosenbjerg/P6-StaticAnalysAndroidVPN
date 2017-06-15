using System;
using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class HighEntropyWordSearchHandler : IConstantStringSearchHandler
    {
        private double _ent;

        public HighEntropyWordSearchHandler(double lowestInterestingEntropy)
        {
            _ent = lowestInterestingEntropy;
        }

        public string OutputName { get; } = "HighEntropyWords";

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            var stringSearchResults = results.Where(u => u.SampleLine.Length > 16 &&
                                                         (u.SampleLine.IndexOf(" ", StringComparison.Ordinal) ==
                                                          -1 ||
                                                          u.SampleLine.IndexOf(" ", StringComparison.Ordinal) >
                                                          15) &&
                                                         !u.SampleLine.Contains("java") &&
                                                         !u.SampleLine.Contains("system") &&
                                                         !u.SampleLine.Contains("cordova") &&
                                                         !u.SampleLine.Contains("Lorg") &&
                                                         !u.SampleLine.Contains("android") &&
                                                         !u.SampleLine.Contains("facebook"))
                .Distinct(new Use.UseComparer());

            return stringSearchResults
                .Where(x => !x.SampleLine.Contains(" ") && !x.SampleLine.Contains("abcdefghijklmnopqrstuvwxyz"))
                .Select(x => new EntropyResult(x.SampleLine, GetEntropy(x.SampleLine), x.File, x.Line, x.Index))
                .Where(x => x.Entropy >= _ent)
                .OrderByDescending(x => x.Entropy)
                .Cast<object>()
                .ToList();
        }

        private static double GetEntropy(string s)
        {
            var map = new Dictionary<char, int>();
            foreach (var c in s)
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;

            var result = 0.0;
            var len = s.Length;
            foreach (var item in map)
            {
                var frequency = (double) item.Value / len;
                result -= frequency * (Math.Log(frequency) / Math.Log(2));
            }

            return result;
        }
    }
}