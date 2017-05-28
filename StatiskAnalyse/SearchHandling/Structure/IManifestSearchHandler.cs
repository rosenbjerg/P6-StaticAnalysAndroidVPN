using System.Collections.Generic;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public interface IManifestSearchHandler
    {
        string OutputName { get; }

        List<object> Process(string xmlTreeLines);
    }
}