using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StatiskAnalyse
{
    public class TrackerSearchHandler : StructureSearchHandler
    {
        internal static readonly string[] Trackers = File.ReadLines("../../trackers.txt").ToArray();
        public override string OutputName { get; } = "Trackers";

        public override List<object> Process(ClassFileDirectory rootDir)
        {
            var retVal = new List<object>();
            foreach (var tracker in Trackers)
            {
                var found = false;
                var tt = tracker.Split('/');
                var root = rootDir;
                for (var i = 0; i < tt.Length; i++)
                {
                    var s = tt[i];
                    if (i == tt.Length - 1)
                        if (root.Directories.Any(d => d.Name == s))
                            found = true;
                        else
                            break;
                    var ro = root.Directories.FirstOrDefault(d => d.Name == s);
                    if (ro != null)
                        root = ro;
                }
                if (found)
                    retVal.Add(tracker);
            }
            return retVal;
        }
    }
}