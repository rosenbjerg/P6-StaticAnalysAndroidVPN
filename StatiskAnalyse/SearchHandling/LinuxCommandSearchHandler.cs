using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class LinuxCommandSearchHandler : IConstantStringSearchHandler
    {
        // NOT IN USE ANY LONGER

        private readonly IEnumerable<string> _commands;

        public LinuxCommandSearchHandler(IEnumerable<string> commands)
        {
            _commands = commands;
        }

        public string OutputName { get; } = "LinuxCommands";

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            return results
                .Where(x => _commands.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " ") || x.SampleLine.Contains(" " + y + " ")))
                .Cast<object>()
                .ToList();
        }
    }
}