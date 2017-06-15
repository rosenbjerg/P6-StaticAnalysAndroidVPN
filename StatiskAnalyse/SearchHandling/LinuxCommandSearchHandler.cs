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

        public List<object> Process(ApkAnalysis apk, IEnumerable<Use> results)
        {
            const string jsb = "Ljava/lang/StringBuilder";
            var cmds = results.Where(x => _commands.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " "))).ToList();
            foreach (var result in cmds)
            {
                // If appended to a stringbuilder
                if (result.FoundIn.Source[result.Line + 2].Contains(jsb + ";->append"))
                {
                    //AnalysisTools.TraceStringBuilder(apk, result.Line);
                }
                // If used as a string
                else
                {
                    
                }

            }
            // TODO More verification of the string actually being used with Runtime->exec
            return null;
        }
    }
}