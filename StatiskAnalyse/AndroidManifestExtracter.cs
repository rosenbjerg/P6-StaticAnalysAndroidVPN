using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StatiskAnalyse
{
    internal class AndroidManifestExtracter
    {

        public static string ExtractXmlTree(string apkPath, string outPath)
        {
            var cmd = $"dump xmltree \"{apkPath}\" AndroidManifest.xml";
            var pstart = new ProcessStartInfo(ApkAnalysis.AaptPah)
            {
                WorkingDirectory = outPath,
                Arguments = cmd,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var p = Process.Start(pstart);
            var text = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            File.WriteAllText(Path.Combine(outPath, "AndroidManifest.xml"), text);
            return text;
        }
    }
}