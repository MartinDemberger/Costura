using System.IO;
using Mono.Cecil;

public partial class InnerTask
{
    public ModuleDefinition Module { get; set; }


    FileStream GetSymbolReaderProvider(string targetPath)
    {
        var pdbPath = Path.ChangeExtension(targetPath, "pdb");
        if (File.Exists(pdbPath))
        {
            logger.LogMessage(string.Format("\tFound debug symbols at '{0}'", pdbPath));
            return File.OpenRead(pdbPath);
        }
        var mdbPath = Path.ChangeExtension(targetPath, "mdb");

        if (File.Exists(mdbPath))
        {
            logger.LogMessage(string.Format("\tFound debug symbols at '{0}'", mdbPath));
            return File.OpenRead(mdbPath);
        }

        logger.LogMessage("\tFound no debug symbols");
        return null;
    }

    public void ReadModule()
    {
        using (var symbolStream = GetSymbolReaderProvider(TargetPath))
        {
            var readSymbols = symbolStream != null;
            var readerParameters = new ReaderParameters
                                       {
                                           AssemblyResolver = assemblyResolver,
                                           ReadSymbols = readSymbols,
                                           SymbolStream = symbolStream,
                                       };
            Module = ModuleDefinition.ReadModule(TargetPath, readerParameters);
        }
    }
}