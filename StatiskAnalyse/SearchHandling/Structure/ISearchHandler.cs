using System.Collections.Generic;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public interface ISearchHandler
    {
        string OutputName { get; }

        List<object> Process(ApkAnalysis apk, IEnumerable<Use> results);
    }
}