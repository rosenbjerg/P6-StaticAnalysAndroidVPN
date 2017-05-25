using System.Collections.Generic;
using System.Linq;

namespace StatiskAnalyse
{
    class LinuxCommandSearchHandler : ConstantStringSearchHandler
    {
        public override string OutputName { get; } = "LinuxCommands";
        public override List<object> Process(IEnumerable<Use> results)
        {
            return results.Where(x => ApkAnalysis.LinuxCommandList.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " "))).Cast<object>().ToList();
        }
    }
}