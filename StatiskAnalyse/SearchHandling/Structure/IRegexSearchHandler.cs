using System.Text.RegularExpressions;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public interface IRegexSearchHandler : ISearchHandler
    {
        Regex Regex { get; }
    }
}