using System.Collections.Generic;

namespace StatiskAnalyse.ResultWrappers
{
    public class SearchResult : IResult
    {
        public string Pattern { get; set; }
        public int UseCount => Uses.Count;
        public List<Use> Uses { get; set; } = new List<Use>();

        public override string ToString()
        {
            return $"{Pattern} : {Uses.Count} uses";
        }
    }
}