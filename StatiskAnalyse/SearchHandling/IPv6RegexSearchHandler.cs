using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse.SearchHandling
{
    internal class IPv6RegexSearchHandler : IPv4RegexSearchHandler
    {
        public override List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results.Where(u => IPAddress.TryParse(u.SampleLine, out IPAddress ip)).Select(i => new IpResult
            {
                IP = i.SampleLine,
                Country = GetCountry(i.SampleLine),
                File = i.File,
                Line = i.Line,
                Index = i.Index
            }).Cast<object>().ToList();
        }

        public IPv6RegexSearchHandler()
        {
            OutputName = "IPv6";
            Regex = new Regex(
                "(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))\r\n",
                RegexOptions.Compiled);
        }
    }
}