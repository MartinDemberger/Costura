using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;


[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FileChangedChecker
{
    Logger logger;
    InnerTask innerTask;

    [ImportingConstructor]
    public FileChangedChecker(Logger logger, InnerTask innerTask)
    {
        this.logger = logger;
        this.innerTask = innerTask;
    }

    public bool ShouldStart()
    {
        if (innerTask.Module.Types.Any(x => x.Name == "ProcessedByCostura"))
        {
            logger.LogMessage("\tDid not process because file has already been processed");
            return false;
        }
        innerTask.Module.Types.Add(new TypeDefinition(null, "ProcessedByCostura", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        return true;
    }
}