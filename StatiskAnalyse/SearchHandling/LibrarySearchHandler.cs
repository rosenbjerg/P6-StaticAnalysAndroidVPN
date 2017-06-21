using System.Collections.Generic;
using System.Linq;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse.SearchHandling
{
    public class LibrarySearchHandler : IStructureSearchHandler
    {
        public static string[] CriticalLibs { get; set; } =
        {
            "org/spongycastle",
            "org/bouncycastle",
            "de/blinkt/openvpn",
            "okhttp3",
            "okhttp",
            "javax"
        };

        public string OutputName { get; } = "Libraries";

        public List<object> Process(ClassFileDirectory apkRoot)
        {
            var retVal = new List<object>();
            foreach (var libs in CriticalLibs)
            {
                var found = false;
                var tt = libs.Split('/');
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
                    retVal.Add(libs);
            }
            return retVal;
        }
    }
}