using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatiskAnalyse.ResultWrappers;
using StatiskAnalyse.SearchHandling.Structure;

namespace StatiskAnalyse
{
    public class ClassFileDirectory
    {
        public string Name { get; private set; }
        public string DirPath { get; set; }
        public List<ClassFileDirectory> Directories { get; } = new List<ClassFileDirectory>();
        public List<ClassFile> Files { get; } = new List<ClassFile>();

        public ClassFile FindFile(string filepath)
        {
            var ret = Files.FirstOrDefault(f => f.FilePath.Contains(filepath));
            if (ret != null)
                return ret;
            foreach (var dir in Directories)
            {
                ret = dir.FindFile(filepath);
                if (ret != null)
                    return ret;
            }
            return null;
        }

        public static ClassFileDirectory LoadFromDirectory(string rootDir, string ext)
        {
            var retVal = new ClassFileDirectory
            {
                Name = new DirectoryInfo(rootDir).Name,
                DirPath = rootDir
            };
            var dirs = Directory.EnumerateDirectories(rootDir);
            retVal.Directories.AddRange(dirs.Select(d => LoadFromDirectory(d, ext)));

            var files = Directory.EnumerateFiles(rootDir, "*." + ext);
            retVal.Files.AddRange(files.Select(ClassFile.FromPath));
            
            return retVal;
        }
        
        public IEnumerable<Use> FindUses(Regex pattern)
        {
            return FindUsesInDir(this, pattern);
        }
        

        private static IEnumerable<Use> FindUsesInDir(ClassFileDirectory dir, Regex pattern)
        {
            var retVal = new List<Use>();
            retVal.AddRange(dir.Directories.SelectMany(d => FindUsesInDir(d, pattern)));
            retVal.AddRange(dir.Files.SelectMany(f => FindOccurencesInString(f, pattern)));
            return retVal;
        }

        private static IEnumerable<Use> FindOccurencesInString(ClassFile cf, Regex searchFor)
        {
            for (var i = 0; i < cf.Source.Length; i++)
            {
                var l = cf.Source[i];
                var m = searchFor.Matches(l);
                if (m.Count == 0) continue;
                foreach (Match match in m)
                    yield return new Use(cf, i + 1, match.Index + 1, match.Value);
            }
        }


        public override string ToString()
        {
            return Name;
        }

        public List<Tuple<string, List<object>>> FindUses(ApkAnalysis apk, List<IRegexSearchHandler> lookFor)
        {
            return lookFor.AsParallel()
                .Select(p => new Tuple<string, List<object>>(p.OutputName, p.Process(apk, FindUsesInDir(this, p.Regex))))
                .ToList();
        }
    }
}