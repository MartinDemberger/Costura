using System;
using System.ComponentModel.Composition;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ResourceCaseFixer
{
    readonly InnerTask innerTask;

    [ImportingConstructor]
    public ResourceCaseFixer(DependencyFinder dependencyFinder, Logger logger, InnerTask innerTask)
    {
        this.innerTask = innerTask;
    }

    public void Execute()
    {
        foreach (var resource in innerTask.Module.Resources)
        {
            if (resource.Name.StartsWith("costura.", StringComparison.InvariantCultureIgnoreCase))
            {
                resource.Name = resource.Name.ToLowerInvariant();
            }
        }
    }
}