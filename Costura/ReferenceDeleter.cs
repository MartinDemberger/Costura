using System;
using System.Collections.Generic;
using System.IO;

public partial class InnerTask
{

    public void DeleteTheReferences()
    {
        if (!DeleteReferences)
        {
            return;
        }
        foreach (var fileToDelete in GetFileToDelete())
        {
            try
            {
                logger.LogMessage(string.Format("\tDeleting '{0}'", fileToDelete));
                File.Delete(fileToDelete);
            }
            catch (Exception exception)
            {
                logger.LogWarning(string.Format("\tTried to delete '{0}' but could not due to the following exception: {1}", fileToDelete, exception));
            }
        }
    }

    IEnumerable<string> GetFileToDelete()
    {
        var directoryName = Path.GetDirectoryName(TargetPath);
        foreach (var dependency in Dependencies)
        {
            var dependencyBinary = Path.Combine(directoryName, Path.GetFileName(dependency));
            if (File.Exists(dependencyBinary))
            {
                yield return dependencyBinary;
            }

            var xmlFile = Path.ChangeExtension(dependencyBinary, "xml");
            if (File.Exists(xmlFile))
            {
                yield return xmlFile;
            }

            var pdbFile = Path.ChangeExtension(dependencyBinary, "pdb");
            if (File.Exists(pdbFile))
            {
                yield return pdbFile;
            }
        }
    }
}