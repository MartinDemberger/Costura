using System.ComponentModel.Composition;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ModuleWriter
{
    Logger logger;
    InnerTask config;

    [ImportingConstructor]
    public ModuleWriter(Logger logger, InnerTask config)
    {
        this.logger = logger;
        this.config = config;
    }

    static ISymbolWriterProvider GetSymbolWriterProvider(string targetPath)
    {
        var pdbPath = Path.ChangeExtension(targetPath, "pdb");
        if (File.Exists(pdbPath))
        {
            return new PdbWriterProvider();
        }
        var mdbPath = Path.ChangeExtension(targetPath, "mdb");

        if (File.Exists(mdbPath))
        {
            return new MdbWriterProvider();
        }
        return null;
    }

    public void Execute()
    {
        Execute(config.TargetPath);
    }

    public void Execute(string targetPath)
    {
        if (config.StrongNameKeyPair == null)
        {
            logger.LogMessage(string.Format("\tSaving assembly to '{0}'.", targetPath));
        }
        else
        {
            logger.LogMessage(string.Format("\tSigning and saving assembly to '{0}'.", targetPath));
        }
        var parameters = new WriterParameters
                             {
                                 StrongNameKeyPair = config.StrongNameKeyPair,
                                 WriteSymbols = true,
                                 SymbolWriterProvider = GetSymbolWriterProvider(config.TargetPath)
                             };
        config.Module.Write(targetPath, parameters);
    }
}