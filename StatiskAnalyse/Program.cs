using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling;

namespace StatiskAnalyse
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GoogleSearch.ApiKey = "";
            ApkAnalysis.SavePath = "R:\\test2";
            var ApkFolder = "C:\\Users\\Malte\\Desktop\\apks";
            
            var linuxCmds = new[]
            {
                "chown",
                "chmod",
                "mkdir",
                "rm",
                "mv",
                "cat",
                "ps",
                "su",
                "sudo",
                "kill",
                "logcat",
                "grep",
                "ls"
            };
            
            var javaMethods = new[]
            {
                "Ljava/security/SecureClassLoader;->defineClass",
                "Ljava/net/URLClassLoader;->defineClass",
                "Landroid/accounts/AccountManager;->get",
                "Ljava/lang/Runtime;->exec"
            };

            var handlers = new SearchHandlerContainer
            {
                RegexSearchHandlers =
                {
                    new IPv4RegexSearchHandler(),
                    //new IPv6RegexSearchHandler(),
                    new UrlRegexSearchHandler(),
                    new StringRegexSearchHandler("JavaMethods", javaMethods)
                },
                ConstantStringSearchHandlers =
                {
                    new HighEntropyWordSearchHandler(4.75),
                    //new HighEntropyWordWGoogleSearchHandler(0, 0, 4.75),
                    new LinuxCommandSearchHandler(linuxCmds)
                },
                ManifestSearchHandlers =
                {
                    new PermissionSearchHandler()
                },
                StructureSearchHandlers =
                {
                    new TrackerSearchHandler(),
                    new LibrarySearchHandler()
                }
            };
            var apks = Directory.EnumerateFiles(ApkFolder, "*.apk")
                .Where(x => x.Contains("Hotspot Shield Free") || x.Contains("Amaze") || x.Contains("Tunnel") ||
                            x.Contains("Ultrasurf"));

            PerformAnalysis(apks, handlers);
            Console.ReadKey();
        }

        private static void PerformAnalysis(IEnumerable<string> apks, SearchHandlerContainer handlers)
        {
            int done = 0, total = apks.Count();
            var tot = 100.0 / total;
            var starttime = DateTime.UtcNow;
            Console.WriteLine($"0/{total} - 0%");
            foreach (var apk in apks)
                try
                {
                    ApkAnalysis.ProcessApk(apk, handlers);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading APK: " + apk);
                }
                finally
                {
                    done++;
                    var timeUsed = DateTime.UtcNow.Subtract(starttime).TotalSeconds;
                    var tpa = timeUsed / done;
                    var tl = (total - done) * tpa / 60;
                    Console.Clear();
                    Console.WriteLine($"{done}/{total} - {Math.Round(tot * done),1}%");
                    Console.WriteLine("Estimated time left: " + Math.Round(tl) + " minutes");
                }
            Console.Clear();
            Console.WriteLine("\nDone with operations");
            var elapsed = DateTime.UtcNow.Subtract(starttime);
            Console.WriteLine($"Elapsed: {elapsed.Minutes} minutes and {elapsed.Seconds} seconds");
        }
    }
}