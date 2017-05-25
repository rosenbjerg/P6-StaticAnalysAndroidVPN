using System.Collections.Generic;

namespace StatiskAnalyse
{
    public abstract class SearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(IEnumerable<Use> results);
    }
}