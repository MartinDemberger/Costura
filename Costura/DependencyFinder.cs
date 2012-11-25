using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Build.Framework;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class DependencyFinder
{
    InnerTask embedTask;
    BuildEnginePropertyExtractor buildEnginePropertyExtractor;
    public List<string> Dependencies;

    [ImportingConstructor]
    public DependencyFinder(InnerTask embedTask, IBuildEngine buildEngine, BuildEnginePropertyExtractor buildEnginePropertyExtractor)
    {
        this.embedTask = embedTask;
        this.buildEnginePropertyExtractor = buildEnginePropertyExtractor;
    }

    public void Execute()
    {
        if (embedTask.ReferenceCopyLocalPaths == null)
        {
            Dependencies = buildEnginePropertyExtractor.GetEnvironmentVariable("ReferenceCopyLocalPaths", false)
                .Where(x => x.EndsWith(".dll") || x.EndsWith(".exe"))
                .ToList();
            return;
        }
        Dependencies = embedTask.ReferenceCopyLocalPaths;
    }
}