using System.Linq;
using Mono.Cecil;


public partial class InnerTask
{

    public bool ShouldStart()
    {
        if (Module.Types.Any(x => x.Name == "ProcessedByCostura"))
        {
            logger.LogMessage("\tDid not process because file has already been processed");
            return false;
        }
        Module.Types.Add(new TypeDefinition(null, "ProcessedByCostura", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        return true;
    }
}