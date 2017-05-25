using System.Text.RegularExpressions;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public abstract class RegexSearchHandler : SearchHandler
    {
        protected RegexSearchHandler(Regex regex)
        {
            Regex = regex;
        }

        public Regex Regex { get; }
    }
}