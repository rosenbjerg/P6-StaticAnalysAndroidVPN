using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse
{
    internal class ApkAnalysis
    {
        internal static readonly string[] Trackers = File.ReadLines("../../trackers.txt").ToArray();
        public static string BakSmaliPath = Path.GetFullPath("../../TOOLS/baksmali-2.2.1.jar");
        public static string AaptPah = "../../TOOLS/aapt.exe";
        public static string SavePath = Path.GetFullPath("/STAN");

        private static readonly Regex StringConstantRegex = new Regex("\".+\"", RegexOptions.Compiled);
        

        public string ManifestXmlTree { get; set; }
        public ClassFileDirectory Root { get; private set; }
        public string Name { get; private set; }
        
        private void Clear()
        {
            Root.Directories.Clear();
            Root.Files.Clear();
        }

        public static void ProcessApk(string path, SearchHandlerContainer container)
        {
            var aa = InternalSmaliToolChain(path);
            var stringConstants = aa.Root.FindUses(StringConstantRegex).FirstOrDefault().Uses;
            var tu = aa.Root.FindUses(container.RegexSearchHandlers);
            foreach (var tuple in tu)
                SaveFile(aa.Name, tuple);
            if (container.ManifestSearchHandlers.Count != 0)
            {
                var ntu = container.ManifestSearchHandlers.AsParallel().Select(
                    t => new Tuple<string, object>(t.OutputName, t.Process(aa.ManifestXmlTree)));
                foreach (var tuple in ntu)
                    SaveFile(aa.Name, tuple);
            }
            if (container.StructureSearchHandlers.Count != 0)
            {
                var ntu = container.StructureSearchHandlers.AsParallel().Select(
                    t => new Tuple<string, object>(t.OutputName, t.Process(aa.Root)));
                foreach (var tuple in ntu)
                    SaveFile(aa.Name, tuple);
            }
            if (container.ConstantStringSearchHandlers.Count != 0)
            {
                var ntu = container.ConstantStringSearchHandlers.AsParallel().Select(
                    t => new Tuple<string, object>(t.OutputName, t.Process(stringConstants)));
                foreach (var tuple in ntu)
                    SaveFile(aa.Name, tuple);
            }
            SaveFile(aa.Name, new Tuple<string, object>("StringConstants", stringConstants));
            aa.Clear();
        }

        private static void SaveFile(string apk, Tuple<string, List<object>> tuple)
        {
            File.WriteAllText(Path.Combine(SavePath, apk, tuple.Item1 + ".json"),
                JsonConvert.SerializeObject(tuple.Item2, Formatting.Indented));
        }

        private static void SaveFile(string apk, Tuple<string, object> tuple)
        {
            File.WriteAllText(Path.Combine(SavePath, apk, tuple.Item1 + ".json"),
                JsonConvert.SerializeObject(tuple.Item2, Formatting.Indented));
        }


        private static ApkAnalysis InternalSmaliToolChain(string path)
        {
            Directory.CreateDirectory(SavePath);
            var aa = new ApkAnalysis {Name = Path.GetFileNameWithoutExtension(path)};
            var dir = Path.Combine(SavePath, aa.Name);
            var o = Path.Combine(dir, "out");
            Directory.CreateDirectory(dir);

            aa.ManifestXmlTree = AndroidManifestExtracter.ExtractXmlTree(path, dir);
            if (!Directory.Exists(o))
                BakSmali(path, o);

            aa.Root = ClassFileDirectory.LoadFromDirectory(o, "smali");
            return aa;
        }
        
        private static void BakSmali(string inputDex, string output)
        {
            var cmd = $"-jar \"{BakSmaliPath}\" disassemble \"{inputDex}\" -o \"{output}\"";
            var pstart = new ProcessStartInfo("java")
            {
                WorkingDirectory = output,
                Arguments = cmd,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var p = Process.Start(pstart);
            p?.WaitForExit();
        }
    }
}