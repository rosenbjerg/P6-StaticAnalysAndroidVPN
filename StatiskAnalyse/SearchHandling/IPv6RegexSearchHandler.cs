using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class IPv6RegexSearchHandler : IRegexSearchHandler
    {
        private static readonly WebClient _wc = new WebClient();
        
        public string OutputName { get; } = "IPv6";

        public List<object> Process(IEnumerable<Use> results)
        {
            var ips = results.Where(u => IPAddress.TryParse(u.SampleLine, out IPAddress ip));
            return ips.Select(i => (object)new IpSearchResult(i.SampleLine, GetCountry(i.SampleLine), i.File, i.Line,
                i.Index)).Distinct().ToList();
        }

        private static string GetCountry(string ip)
        {
            var json = _wc.DownloadString("https://freegeoip.net/json/" + ip);
            var deez = JsonConvert.DeserializeObject<FreeGeoIpResponse>(json);
            return deez.country_name;
        }

        public Regex Regex { get; } = new Regex(
            "(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))\r\n",
            RegexOptions.Compiled);
    }
}