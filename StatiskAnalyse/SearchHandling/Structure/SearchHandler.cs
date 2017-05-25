using System.Collections.Generic;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public abstract class SearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(IEnumerable<Use> results);
    }
}