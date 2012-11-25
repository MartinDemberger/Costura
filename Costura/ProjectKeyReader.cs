using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public partial class InnerTask
{

    public StrongNameKeyPair StrongNameKeyPair { get; set; }

    static string GetKeyFile(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        return (
                   from c in xDocument.BuildDescendants("AssemblyOriginatorKeyFile")
                   select c.Value)
            .FirstOrDefault();
    }

    static bool IsSignAssemblyTrue(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        var signAssembly = (
                               from c in xDocument.BuildDescendants("SignAssembly")
                               select c.Value)
            .FirstOrDefault();
        return string.Equals(signAssembly, "true", StringComparison.OrdinalIgnoreCase);
    }

    public void ReadProjectKey()
    {
        if (KeyFilePath == null)
        {
            var projectFilePath = GetProjectPath();
            if (!IsSignAssemblyTrue(projectFilePath))
            {
                return;
            }

            var assemblyOriginatorKeyFile = GetKeyFile(projectFilePath);
            if (assemblyOriginatorKeyFile == null)
            {
                return;
            }
            KeyFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath), assemblyOriginatorKeyFile);
        }

        StrongNameKeyPair = new StrongNameKeyPair(File.OpenRead(KeyFilePath));
    }
}