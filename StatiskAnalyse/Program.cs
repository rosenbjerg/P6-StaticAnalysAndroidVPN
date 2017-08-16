using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StatiskAnalyse.SearchHandling;

namespace StatiskAnalyse
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            HighEntropyWordWGoogleSearchHandler.ApiKey = "";
            ApkAnalysis.SavePath = "C:\\test";
            var ApkFolder = "C:\\apks";
            
            var handlers = new SearchHandlerContainer
            {
                RegexSearchHandlers =
                {
                    new IPv4RegexSearchHandler(),
                    //new IPv6RegexSearchHandler(),
                    //new UrlRegexSearchHandler(),
                    //new ClassLoaderUsageSearchHandler(),
                    //new ExecutedCommandSearchHandler(),
                    new ReflectionSetAccesabillitySearchHandler(),
                    new reflectionSearchHandler(),

                },
                ConstantStringSearchHandlers =
                {
                    new HighEntropyWordSearchHandler(4.75),
                    //new HighEntropyWordWGoogleSearchHandler(0, 0, 4.75),
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
            var apks = Directory.EnumerateFiles(ApkFolder, "*.apk");
                //.Where(x => x.Contains("Amaze"));
            //.Take(3);
            //.Where(x => x.Contains("Hotspot Shield Free") || x.Contains("Amaze") || x.Contains("Fly") || x.Contains("Ghost"));

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
                    var timeUsed = DateTime.UtcNow.Subtract(starttime);
                    var tpa = timeUsed.TotalSeconds / done;
                    var tl = (total - done) * tpa;
                    Console.Clear();
                    Console.WriteLine($"{done}/{total} - {Math.Round(tot * done),1}%");
                    if (tl > 60)
                        Console.WriteLine("Estimated time left: " + Math.Round(tl / 60) + " minutes and " + Math.Round(tl % 60) + " seconds");
                    else
                        Console.WriteLine("Estimated time left: " + Math.Round(tl) + " seconds");
                }
            Console.Clear();
            Console.WriteLine("\nDone with operations");
            var elapsed = DateTime.UtcNow.Subtract(starttime);
            Console.WriteLine($"Elapsed: {elapsed.Minutes} minutes and {elapsed.Seconds} seconds");
            Console.WriteLine($"{Math.Round(elapsed.TotalSeconds / total, 1)} seconds per app on average");
        }

    }
}