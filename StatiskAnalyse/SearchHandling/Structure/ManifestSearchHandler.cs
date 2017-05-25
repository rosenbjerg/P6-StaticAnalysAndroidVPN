using System.Collections.Generic;

namespace StatiskAnalyse
{
    public abstract class ManifestSearchHandler
    {
        public abstract string OutputName { get; }

        public abstract List<object> Process(IEnumerable<string> xmlTreeLines);
    }

    public class PermissionSearchHandler : ManifestSearchHandler
    {
        public override string OutputName { get; } = "Permissions";

        public override List<object> Process(IEnumerable<string> xmlTreeLines)
        {
            throw new System.NotImplementedException();
        }
    }
}