using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Core.Extensions;
using Coevery.Entities.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Entities.Extensions {
    public class ContentDefinitionExtension : IContentDefinitionExtension {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ContentDefinitionExtension(
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager) {
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions() {
            var metaEntities = _contentManager.Query<EntityMetadataPart>(VersionOptions.Published)
                                              .List();
            if (metaEntities == null || !metaEntities.Any()) {
                return null;
            }
            return _contentDefinitionManager.ListTypeDefinitions()
                                            .Where(type => (metaEntities.Any(entity => string.Equals(entity.Name, type.Name, StringComparison.Ordinal))));
        }

        public IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions() {
            var types = ListUserDefinedTypeDefinitions();
            if (types == null) {
                return null;
            }
            var result = types.SelectMany(type =>
                                          type.Parts.Where(part => string.Equals(part.PartDefinition.Name, type.Name.ToPartName(), StringComparison.Ordinal))
                                              .Select(part => part.PartDefinition));
            return result.Any() ? result : null;
        }
    }
}