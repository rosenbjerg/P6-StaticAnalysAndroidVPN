using System.Collections.Generic;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public interface IStructureSearchHandler
    {
        string OutputName { get; }

        List<object> Process(ClassFileDirectory apkRoot);
    }
    
}