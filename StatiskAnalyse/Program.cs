using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse
{
    public class SearchHandlerContainer
    {
        public List<RegexSearchHandler> RegexSearchHandlers { get; } = new List<RegexSearchHandler>();

        public List<ConstantStringSearchHandler> ConstantStringSearchHandlers { get; } =
            new List<ConstantStringSearchHandler>();

        public List<ManifestSearchHandler> ManifestSearchHandlers { get; } = new List<ManifestSearchHandler>();
        public List<StructureSearchHandler> StructureSearchHandlers { get; } = new List<StructureSearchHandler>();
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            // test key
            GoogleSearch.ApiKey = "AIzaSyDrqFQq2jnMtCtiNPiI5D6KDCWJT_Fyrt4";
            ApkAnalysis.SavePath = "R:\\test2";
            var ApkFolder = "C:\\Users\\Malte\\Desktop\\apks";
            //string ApkFolder = "/folder/with/apk/files";

            if (args.Length > 0 && Directory.Exists(args[0]))
                ApkFolder = args[0];

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
                    new UrlRegexSearchHandler(),
                    new StringRegexSearchHandler("JavaMethods", javaMethods)
                },
                ConstantStringSearchHandlers =
                {
                    new HighEntropyWordSearchHandler(4.75),
                    new HighEntropyWordWGoogleSearchHandler(0, 0, 4.75),
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

            PerformAnalysis(ApkFolder, handlers);
            Console.ReadKey();
        }

        private static void PerformAnalysis(string apkFolder, SearchHandlerContainer handlers)
        {
            var apks = Directory.EnumerateFiles(apkFolder, "*.apk")
                .Where(x => x.Contains("Hotspot Shield Free") || x.Contains("Amaze") || x.Contains("Tunnel") ||
                            x.Contains("Ultrasurf"));
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