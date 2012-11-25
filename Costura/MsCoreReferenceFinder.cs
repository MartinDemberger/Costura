using System.Linq;
using Mono.Cecil;

public partial class InnerTask
{
    public TypeReference VoidTypeReference;

    public void FindMsCoreReferences()
    {
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            FindWinRTMsCoreReferences();
            return;
        }

        var voidDefinition = msCoreTypes.First(x => x.Name == "Void");
        VoidTypeReference = Module.Import(voidDefinition);


    }

    public void FindWinRTMsCoreReferences()
    {
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;
        var voidDefinition = systemRuntimeTypes.First(x => x.Name == "Void");
        VoidTypeReference = Module.Import(voidDefinition);
    }


}