using System.Collections.Generic;

namespace StatiskAnalyse.SearchHandling.Structure
{
    public abstract class StructureSearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(ClassFileDirectory rootDir);
    }
    
}