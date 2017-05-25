using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace StatiskAnalyse
{
    class UrlRegexSearchHandler : RegexSearchHandler
    {
        public UrlRegexSearchHandler() : base(new Regex("https?:\\/\\/([\\da-z\\.-]+)\\.([a-z\\.]{2,6})([\\/\\w \\.-]*)*\\/?", RegexOptions.Compiled))
        {

        }

        public override string OutputName { get; } = "Urls";

        public override List<object> Process(IEnumerable<Use> results)
        {
            return results.Cast<object>().ToList();
        }
        
    }
}