using System.Collections.Generic;
using System.Linq;

public partial class InnerTask
{
    public List<string> Dependencies;


    public void FindDependencies()
    {
        if (ReferenceCopyLocalPaths == null)
        {
            Dependencies = GetEnvironmentVariable("ReferenceCopyLocalPaths", false)
                .Where(x => x.EndsWith(".dll") || x.EndsWith(".exe"))
                .ToList();
            return;
        }
        Dependencies = ReferenceCopyLocalPaths;
    }
}