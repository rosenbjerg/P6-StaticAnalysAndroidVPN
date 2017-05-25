using System.Collections.Generic;

namespace StatiskAnalyse
{
    public abstract class StructureSearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(ClassFileDirectory rootDir);
    }
}