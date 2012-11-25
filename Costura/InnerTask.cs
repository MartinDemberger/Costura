using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;

public partial class InnerTask
{
    public bool Overwrite;
    public bool IncludeDebugSymbols;
    public bool DeleteReferences;
    public bool CreateTemporaryAssemblies;

    public  IBuildEngine BuildEngine;
    public string TargetPath;
    public string MessageImportance;
    public string References;
    public Exception Exception;
    public string KeyFilePath;
    //Hack:
    public List<string> ReferenceCopyLocalPaths;
    Logger logger;
    static Version version;
    AssemblyResolver assemblyResolver;

    static InnerTask()
    {
        version = typeof(InnerTask).Assembly.GetName().Version;
    }

    public InnerTask()
    {
        MessageImportance = "Low";
        Overwrite = true;
        DeleteReferences = true;
        IncludeDebugSymbols = true;
    }

    public bool Execute()
    {
        var message = string.Format("Costura.EmbedTask v{0} Executing (Change MessageImportance to get more or less info)", version);
        var buildMessageEventArgs = new BuildMessageEventArgs(message, "", "EmbedTask", Microsoft.Build.Framework.MessageImportance.High);
        BuildEngine.LogMessageEvent(buildMessageEventArgs);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger = new Logger
                {
                    BuildEngine = BuildEngine,
                };
            logger.Initialise(MessageImportance);
            Inner();
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
        finally
        {
            stopwatch.Stop();
            logger.Flush();
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("\tFinished ({0}ms)", stopwatch.ElapsedMilliseconds), "", "Costura.EmbedTask", Microsoft.Build.Framework.MessageImportance.High));
        }
        return !logger.ErrorHasBeenRaised;
    }


    void HandleException(Exception exception)
    {
        Exception = exception;
        if (exception is WeavingException)
        {
            logger.LogError(exception.Message);
            return;
        }

        logger.LogError(string.Format("Unhandled exception occurred {0}", exception));
    }


    void Inner()
    {
        using (var catalog = new AssemblyCatalog(GetType().Assembly))
        using (var container = new CompositionContainer(catalog))
        {
            container.ComposeExportedValue(this);
            container.ComposeExportedValue(BuildEngine);
            container.ComposeExportedValue(logger);
            CheckForInvalidConfig();
            container.GetExportedValue<TargetPathFinder>().Execute();

            logger.LogMessage(string.Format("\tTargetPath: {0}", TargetPath));


            assemblyResolver = container.GetExportedValue<AssemblyResolver>();
            assemblyResolver.Execute();
            ReadModule();

            if (!ShouldStart())
            {
                return;
            }

            container.GetExportedValue<MsCoreReferenceFinder>().Execute();

            container.GetExportedValue<AssemblyLoaderImporter>().Execute();
            container.GetExportedValue<ModuleLoaderImporter>().Execute();
            container.GetExportedValue<DependencyFinder>().Execute();
            container.GetExportedValue<ProjectKeyReader>().Execute();
            container.GetExportedValue<ResourceCaseFixer>().Execute();
            using (var resourceEmbedder = container.GetExportedValue<ResourceEmbedder>())
            {
                resourceEmbedder.Execute();
                var savePath = GetSavePath();
                container.GetExportedValue<ModuleWriter>().Execute(savePath);
            }
            container.GetExportedValue<ReferenceDeleter>().Execute();
        }
    }

    void CheckForInvalidConfig()
    {
        if (!Overwrite && DeleteReferences)
        {
            throw new WeavingException("Overwrite=false and DeleteReferences=true is invalid because if the new file is copied to a different directory it serves no purpose deleting references.");
        }
    }

    string GetSavePath()
    {
        var fileInfo = new FileInfo(TargetPath);
        var directoryPath = Path.Combine(fileInfo.DirectoryName, "CosturaMerged");
        //Try to delete directory for cleanup purposes. 
        try
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }
        catch (Exception)
        {
        }
        if (Overwrite)
        {
            return TargetPath;
        }
        Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, fileInfo.Name);
    }
}