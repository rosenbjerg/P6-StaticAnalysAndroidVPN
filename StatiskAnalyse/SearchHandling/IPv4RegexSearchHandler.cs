using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class IPv4RegexSearchHandler : RegexSearchHandler
    {
        private static readonly WebClient _wc = new WebClient();

        private static readonly string[] _dnsServerIps =
        {
            "8.8.8.8",
            "8.8.4.4",
            "8.8.0.0",
            "4.2.2.2",
            "4.2.2.4"
        };

        public IPv4RegexSearchHandler() : base(new Regex("[0-9]{1,3}(\\.[0-9]{1,3}){3}", RegexOptions.Compiled))
        {
        }

        public override string OutputName { get; } = "IPv4";

        public override List<object> Process(IEnumerable<Use> results)
        {
            var ips = results.Where(u => IPAddress.TryParse(u.SampleLine, out IPAddress ip));
            return ips.Select(i => (object) new IpSearchResult(i.SampleLine, GetCountry(i.SampleLine), i.File, i.Line,
                i.Index)).Distinct().ToList();
        }

        private static string GetCountry(string ip)
        {
            if (ip == "127.0.0.1")
                return "localhost";
            if (ip == "0.0.0.0")
                return "all local interfaces";
            if (ip.StartsWith("10."))
                return "local";
            if (_dnsServerIps.Contains(ip))
                return "DNS server";
            var json = _wc.DownloadString("https://freegeoip.net/json/" + ip);
            var deez = JsonConvert.DeserializeObject<FreeGeoIpResponse>(json);
            return deez.country_name;
        }
    }
}