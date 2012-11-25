using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class InnerTask
{
    public void ImportModuleLoader()
    {
        const MethodAttributes attributes = MethodAttributes.Static
                                            | MethodAttributes.SpecialName
                                            | MethodAttributes.RTSpecialName;
        var cctor = GetCctor(attributes);
        var il = cctor.Body.GetILProcessor();
        il.Append(il.Create(OpCodes.Call, AttachMethod));
        il.Append(il.Create(OpCodes.Ret));
    }

    MethodDefinition GetCctor(MethodAttributes attributes)
    {
        var moduleClass = Module.Types.FirstOrDefault(x => x.Name == "<Module>");
        if (moduleClass == null)
        {
            throw new WeavingException("Found no module class!");
        }
        var cctor = moduleClass.Methods.FirstOrDefault(x => x.Name == ".cctor");
        if (cctor == null)
        {
            cctor = new MethodDefinition(".cctor", attributes, VoidTypeReference);
            moduleClass.Methods.Add(cctor);
        }
        return cctor;
    }
}