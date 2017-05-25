using System.Text.RegularExpressions;

namespace StatiskAnalyse
{
    public abstract class RegexSearchHandler : SearchHandler
    {
        public Regex Regex { get; }
        protected RegexSearchHandler(Regex regex)
        {
            Regex = regex;
        }
    }
}