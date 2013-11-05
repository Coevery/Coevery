using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Coevery.Tasks {
    public class FilterModuleBinaries : Task {
        public ITaskItem[] ModulesBinaries { get; set; }
        public ITaskItem[] CoeveryWebBinaries { get; set; }

        [Output]
        public ITaskItem[] ExcludedBinaries { get; set; }

        public override bool Execute() {
            if (ModulesBinaries == null || CoeveryWebBinaries == null)
                return true;

            var coeveryWebAssemblies = new HashSet<string>(
                CoeveryWebBinaries.Select(item => Path.GetFileName(item.ItemSpec)),
                StringComparer.InvariantCultureIgnoreCase);

            ExcludedBinaries = ModulesBinaries
                .Where(item => coeveryWebAssemblies.Contains(Path.GetFileName(item.ItemSpec)))
                .Select(item => new TaskItem(item))
                .ToArray();

            return true;
        }
    }
}
