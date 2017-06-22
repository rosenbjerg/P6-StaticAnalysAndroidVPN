using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class IPv4RegexSearchHandler : IRegexSearchHandler
    {
        protected static readonly WebClient _wc = new WebClient();

        private static readonly string[] _dnsServerIps =
        {
            "8.8.8.8",
            "8.8.4.4",
            "8.8.0.0",
            "4.2.2.2",
            "4.2.2.4"
        };
        
        public string OutputName { get; protected set; } = "IPv4";

        public virtual List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results.Where(u => Regex.IsMatch(u) && IPAddress.TryParse(u.SampleLine, out IPAddress ip)).Select(i => new IpResult
            {
                IP = i.SampleLine,
                Country = GetCountry(i.SampleLine),
                File = i.File,
                Line = i.Line,
                Index = i.Index
            }).Cast<object>().ToList();
        }

        protected static string GetCountry(string ip)
        {
            if (ip == "127.0.0.1")
                return "localhost";
            if (ip == "0.0.0.0")
                return "all local interfaces";
            if (ip.StartsWith("10."))
                return "local";
            if (_dnsServerIps.Contains(ip))
                return "DNS server";
            try
            {
                var json = _wc.DownloadString("https://freegeoip.net/json/" + ip);
                var m = _countryRegex.Match(json);
                if (m.Success)
                    return m.Groups[1].Value;
                return "(Could not check country)";
            }
            catch (WebException)
            {
                return "(Could not check country)";
            }
        }
        protected static Regex _countryRegex = new Regex("\"country_name\":\"([^\"]*)\"", RegexOptions.Compiled);
        public Regex Regex { get; protected set; } = new Regex("[0-9]{1,3}(\\.[0-9]{1,3}){3}", RegexOptions.Compiled);
    }

    class IpResult : FileResultWrapper
    {
        public string IP { get; set; }
        public string Country { get; set; }
    }
}