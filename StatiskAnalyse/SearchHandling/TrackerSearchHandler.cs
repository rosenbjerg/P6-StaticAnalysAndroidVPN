using System.Collections.Generic;
using System.IO;
using System.Linq;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    public class TrackerSearchHandler : IStructureSearchHandler
    {
        internal static readonly string[] Trackers = File.ReadLines("../../trackers.txt").ToArray();

        public string OutputName { get; } = "Trackers";

        public List<object> Process(ClassFileDirectory apkRoot)
        {
            var retVal = new List<object>();
            foreach (var tracker in Trackers)
            {
                var found = false;
                var tt = tracker.Split('/');
                var root = apkRoot;
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