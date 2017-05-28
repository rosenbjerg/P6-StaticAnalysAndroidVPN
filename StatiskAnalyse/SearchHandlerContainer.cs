using System.Collections.Generic;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse
{
    public class SearchHandlerContainer
    {
        public List<IRegexSearchHandler> RegexSearchHandlers { get; } = new List<IRegexSearchHandler>();

        public List<IConstantStringSearchHandler> ConstantStringSearchHandlers { get; } =
            new List<IConstantStringSearchHandler>();

        public List<IManifestSearchHandler> ManifestSearchHandlers { get; } = new List<IManifestSearchHandler>();
        public List<IStructureSearchHandler> StructureSearchHandlers { get; } = new List<IStructureSearchHandler>();
    }
}