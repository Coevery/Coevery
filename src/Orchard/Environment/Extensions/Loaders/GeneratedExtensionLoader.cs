using System.IO;
using System.Linq;
using System.Reflection;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;

namespace Orchard.Environment.Extensions.Loaders
{
    public class GeneratedExtensionLoader : ExtensionLoaderBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IAssemblyProbingFolder _assemblyProbingFolder;

        public GeneratedExtensionLoader(IDependenciesFolder dependenciesFolder,
                                        IVirtualPathProvider virtualPathProvider,
                                        IAssemblyProbingFolder assemblyProbingFolder,
                                        IHostEnvironment hostEnvironment)
            : base(dependenciesFolder) {
            _virtualPathProvider = virtualPathProvider;
            _assemblyProbingFolder = assemblyProbingFolder;
            _hostEnvironment = hostEnvironment;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override int Order
        {
            get { return 200; }
        }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor) {
            //if (descriptor.Id == "Coevery.DynamicTypes")
            //{
            //    var virtualPath = GetAssemblyVirtualPath(descriptor);
            //    return new ExtensionProbeEntry
            //    {
            //        Descriptor = descriptor,
            //        Loader = this,
            //        Priority = 100, // Higher priority because assemblies in ~/bin always take precedence
            //        VirtualPath = virtualPath,
            //        VirtualPathDependencies = Enumerable.Empty<string>(),
            //    };
            //}
            return null;
        }

        public override void ExtensionActivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
            string sourceFileName = _virtualPathProvider.MapPath(GetAssemblyVirtualPath(extension));

            // Copy the assembly if it doesn't exist or if it is older than the source file.
            bool copyAssembly =
                !_assemblyProbingFolder.AssemblyExists(extension.Id) ||
                File.GetLastWriteTimeUtc(sourceFileName) > _assemblyProbingFolder.GetAssemblyDateTimeUtc(extension.Id);

            if (copyAssembly)
            {
                ctx.CopyActions.Add(() => _assemblyProbingFolder.StoreAssembly(extension.Id, sourceFileName));

                // We need to restart the appDomain if the assembly is loaded
                if (_hostEnvironment.IsAssemblyLoaded(extension.Id))
                {
                    Logger.Information("ExtensionRemoved: Module \"{0}\" is activated with newer file and its assembly is loaded, forcing AppDomain restart", extension.Id);
                    ctx.RestartAppDomain = true;
                }
            }
        }

        public override void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
            if (_assemblyProbingFolder.AssemblyExists(extension.Id))
            {
                ctx.DeleteActions.Add(
                    () =>
                    {
                        Logger.Information("ExtensionDeactivated: Deleting assembly \"{0}\" from probing directory", extension.Id);
                        _assemblyProbingFolder.DeleteAssembly(extension.Id);
                    });

                // We need to restart the appDomain if the assembly is loaded
                if (_hostEnvironment.IsAssemblyLoaded(extension.Id))
                {
                    Logger.Information("ExtensionDeactivated: Module \"{0}\" is deactivated and its assembly is loaded, forcing AppDomain restart", extension.Id);
                    ctx.RestartAppDomain = true;
                }
            }
        }

        private string GetAssemblyVirtualPath(ExtensionDescriptor descriptor) {
            var virtualPath = _virtualPathProvider.Combine("~/Modules/Coevery.Dynamic", "bin",
                                                descriptor.Id + ".dll");
            return virtualPath;
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor) {

            var assembly = _assemblyProbingFolder.LoadAssembly(descriptor.Id);
            if (assembly == null)
            {
                var virtualPath = GetAssemblyVirtualPath(descriptor);

                var physicalPath = _virtualPathProvider.MapPath(virtualPath);
                assembly = TryAssemblyLoadFrom(physicalPath);
                if (assembly == null)
                {
                    Logger.Error("Dynamically generated modules cannot be activated because assembly '{0}' could not be loaded", physicalPath);
                    return null;
                }
            }

            Logger.Information("Loaded dynamically generated module \"{0}\": assembly name=\"{1}\"", descriptor.Name, descriptor.Id);

            return new ExtensionEntry {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetExportedTypes()
            };
        }

        private static Assembly TryAssemblyLoadFrom(string assemblyFile)
        {
            try
            {
                return Assembly.LoadFrom(assemblyFile);
            }
            catch
            {
                return null;
            }
        }
    }
}