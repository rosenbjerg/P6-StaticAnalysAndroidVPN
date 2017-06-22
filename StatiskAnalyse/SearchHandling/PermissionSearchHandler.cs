using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    public class PermissionSearchHandler : IManifestSearchHandler
    {
        private static readonly Regex PermissionRegex = new Regex("\"android\\.permission\\.(\\w+)\"");

        public string OutputName { get; } = "Permissions";

        public List<object> Process(string xmlTreeLines)
        {
            var retVal = new List<object>();
            var matches = PermissionRegex.Matches(xmlTreeLines);
            foreach (Match m in matches)
                retVal.Add(m.Groups[1].Value);
            return retVal.Distinct().ToList();
        }
    }
}