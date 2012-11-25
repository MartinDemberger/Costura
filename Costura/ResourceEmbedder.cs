﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ResourceEmbedder : IDisposable
{
    DependencyFinder dependencyFinder;
    InnerTask embedTask;
    List<Stream> streams;
    Logger logger;

    [ImportingConstructor]
    public ResourceEmbedder(DependencyFinder dependencyFinder, InnerTask embedTask, Logger logger)
    {
        streams = new List<Stream>();
        this.dependencyFinder = dependencyFinder;
        this.embedTask = embedTask;
        this.logger = logger;
    }

    public void Execute()
    {
        foreach (var dependency in dependencyFinder.Dependencies)
        {
            var fullPath = Path.GetFullPath(dependency);
            Embedd(fullPath);
            if (!embedTask.IncludeDebugSymbols)
            {
                continue;
            }
            var pdbFullPath = Path.ChangeExtension(fullPath, "pdb");
            if (File.Exists(pdbFullPath))
            {
                Embedd(pdbFullPath);
            }
        }
    }

    void Embedd(string fullPath)
    {
        logger.LogMessage(string.Format("\tEmbedding '{0}'", fullPath));
        var fileStream = File.OpenRead(fullPath);
        streams.Add(fileStream);
        var resource = new EmbeddedResource("costura." + Path.GetFileName(fullPath).ToLowerInvariant(), ManifestResourceAttributes.Private, fileStream);
        embedTask.Module.Resources.Add(resource);
    }

    public void Dispose()
    {
        foreach (var stream in streams)
        {
            stream.Dispose();
        }
    }
}