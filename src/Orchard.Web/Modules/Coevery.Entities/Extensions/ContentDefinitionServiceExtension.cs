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
            var metaEntities = _contentManager.Query<EntityMetadataPart>(VersionOptions.Latest)
                                              .List();
            if (metaEntities == null || !metaEntities.Any()) {
                return null;
            }
            return from type in _contentDefinitionManager.ListTypeDefinitions()
                   from entity in metaEntities
                   where entity.Name == type.Name
                   select type;
        }

        public IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions() {
            var types = ListUserDefinedTypeDefinitions();
            if (types == null || !types.Any()) {
                return null;
            }
            var result = from type in types
                         from partRelation in type.Parts
                         let part = partRelation.PartDefinition
                         where part.Name == type.Name.ToPartName()
                         select part;
            return result.Any() ? result : null;
        }
    }
}