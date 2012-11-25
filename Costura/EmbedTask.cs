using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;

namespace Costura
{
    public class EmbedTask : Task
    {
        public bool Overwrite { set; get; }
        public bool IncludeDebugSymbols { set; get; }
        public bool DeleteReferences { set; get; }
        public bool CreateTemporaryAssemblies { get; set; }

        public string TargetPath { set; get; }
        public string MessageImportance { set; get; }
        public string References { get; set; }
        public Exception Exception { get; set; }
        public string KeyFilePath { get; set; }
        //Hack:
        public List<string> ReferenceCopyLocalPaths { get; set; }
        
        public EmbedTask()
        {
            MessageImportance = "Low";
            Overwrite = true;
            DeleteReferences = true;
            IncludeDebugSymbols = true;
        }

        public override bool Execute()
        {
            var innerTask = new InnerTask
                {
                    Overwrite = Overwrite,
                    IncludeDebugSymbols = IncludeDebugSymbols,
                    DeleteReferences = DeleteReferences,
                    CreateTemporaryAssemblies = CreateTemporaryAssemblies,
                    TargetPath = TargetPath,
                    MessageImportance = MessageImportance,
                    References = References,
                    ReferenceCopyLocalPaths = ReferenceCopyLocalPaths,
                    BuildEngine = BuildEngine
                };
            return innerTask.Execute();
        }
    }

}