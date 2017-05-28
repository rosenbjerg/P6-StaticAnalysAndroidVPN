using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    internal class LinuxCommandSearchHandler : IConstantStringSearchHandler
    {
        private readonly IEnumerable<string> _commands;

        public LinuxCommandSearchHandler(IEnumerable<string> commands)
        {
            _commands = commands;
        }

        public string OutputName { get; } = "LinuxCommands";

        public List<object> Process(IEnumerable<Use> results)
        {
            var cmds = results.Where(x => _commands.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " ")))
                .Cast<object>().ToList();
            // TODO More verification of the string actually being used with Runtime->exec
            return cmds;
        }
    }
}