using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            const string jsinv = ", \\1}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder";
            var cmds = results.Where(x => _commands.Any(y => x.SampleLine == y || x.SampleLine.StartsWith(y + " ") || x.SampleLine.Contains(" " + y + " "))).ToList();
            foreach (var result in cmds)
            {
                var line = result.FoundIn.Source[result.Line - 1];
                var s = line.IndexOf('v');
                var c = line.IndexOf(',') - s;
                var reg = line.Substring(s, c);
                // If appended to a stringbuilder
                if (result.FoundIn.Source[result.Line + 1].Contains(jsinv.Replace("\\1", reg)))
                {
                    line = result.FoundIn.Source[result.Line + 1];
                    s = line.IndexOf('{') + 1;
                    c = line.IndexOf(',') - s;
                    var sbReg = line.Substring(s, c);
                    Console.WriteLine(line);
                    AnalysisTools.TraceStringBuilder(apk, result.FoundIn, result.Line + 1, sbReg);
                }
                // If used as a string
                else
                {
                    
                }

            }
            foreach (var result in cmds)
            {
                for (int i = result.Line; ;i--)
                {
                    if (result.FoundIn.Source[i].StartsWith(".method"))
                    {
                        AnalysisTools.TraceMethodCall(apk,result,i);
                        break;
                    }
                }
            }
            // TODO More verification of the string actually being used with Runtime->exec
            return null;
        }
    }
}