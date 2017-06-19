using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StatiskAnalyse.ResultWrappers;

namespace StatiskAnalyse
{
    public class ApkAnalysis
    {
        internal static readonly string[] Trackers = File.ReadLines("../../trackers.txt").ToArray();
        public static string BakSmaliPath = Path.GetFullPath("../../TOOLS/baksmali-2.2.1.jar");
        public static string AaptPah = "../../TOOLS/aapt.exe";
        public static string SavePath = Path.GetFullPath("/STAN");
        
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
            if (container.RegexSearchHandlers.Count != 0)
                container.RegexSearchHandlers.AsParallel()
                    .ForAll(sh => SaveFile(aa.Name, sh.OutputName, aa.Root.FindUses(aa, sh)));

            if (container.ManifestSearchHandlers.Count != 0)
                container.ManifestSearchHandlers.AsParallel()
                    .ForAll(sh => SaveFile(aa.Name, sh.OutputName, sh.Process(aa.ManifestXmlTree)));

            if (container.StructureSearchHandlers.Count != 0)
                container.StructureSearchHandlers.AsParallel()
                    .ForAll(sh => SaveFile(aa.Name, sh.OutputName, sh.Process(aa)));

            var stringConstants = aa.Root.FindUses(Util.ConstantStringRegex, 3).ToList();
            if (container.ConstantStringSearchHandlers.Count != 0)
                container.ConstantStringSearchHandlers.AsParallel()
                    .ForAll(sh => SaveFile(aa.Name, sh.OutputName, sh.Process(aa, stringConstants)));

            SaveFile(aa.Name, "StringConstants", stringConstants);
            aa.Clear();
        }

        private static void SaveFile<T>(string apk, string name, List<T> results)
        {
            File.WriteAllText(Path.Combine(SavePath, apk, name + ".json"),
                JsonConvert.SerializeObject(results, Formatting.Indented));
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