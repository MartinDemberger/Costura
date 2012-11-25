using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;

public partial class InnerTask
{

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


    public void WriteModule(string targetPath)
    {
        if (StrongNameKeyPair == null)
        {
            logger.LogMessage(string.Format("\tSaving assembly to '{0}'.", targetPath));
        }
        else
        {
            logger.LogMessage(string.Format("\tSigning and saving assembly to '{0}'.", targetPath));
        }
        var parameters = new WriterParameters
                             {
                                 StrongNameKeyPair = StrongNameKeyPair,
                                 WriteSymbols = true,
                                 SymbolWriterProvider = GetSymbolWriterProvider(TargetPath)
                             };
        Module.Write(targetPath, parameters);
    }
}