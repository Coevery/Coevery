using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Environment.Extensions.Folders
{
    public class GeneratedModuleFolders : IExtensionFolders {
        public IEnumerable<ExtensionDescriptor> AvailableExtensions() {

            var extensionDescriptor = new ExtensionDescriptor {
                Id = "Coevery.DynamicTypes",
                ExtensionType = DefaultExtensionTypes.Module,
                Location = "~/Modules",
                Name = "Coevery.DynamicTypes"
            };

            var featureDescriptors = new List<FeatureDescriptor>();

            // Default feature
            var defaultFeature = new FeatureDescriptor {
                Id = extensionDescriptor.Id,
                Name = extensionDescriptor.Name,
                Priority = 0,
                Description = string.Empty,
                Dependencies = Enumerable.Empty<string>(),
                Extension = extensionDescriptor,
                Category = "Dynamic"
            };

            featureDescriptors.Add(defaultFeature);

            extensionDescriptor.Features = featureDescriptors;

            yield return extensionDescriptor;
        }
    }
}