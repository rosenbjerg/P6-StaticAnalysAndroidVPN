using System.Collections.Generic;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public abstract class ManifestSearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(string xmlTreeLines);
    }
}