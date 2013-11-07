using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Entities.Models;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Entities.Services;

namespace Coevery.Entities.Extensions {
    public class ContentDefinitionExtension : IContentDefinitionExtension {
        private readonly IContentManager _contentManager;
        private readonly ISettingService _settingService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ContentDefinitionExtension(
            IContentManager contentManager,
            ISettingService settingService,
            IContentDefinitionManager contentDefinitionManager) {
            _settingService = settingService;
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

        public EntityNames GetEntityNames(string entityName) {
            var entity = _contentDefinitionManager.GetTypeDefinition(entityName);
            if (entity == null) {
                return null;
            }
            var setting = entity.Settings;
            return setting.ContainsKey("CollectionName") && setting.ContainsKey("CollectionDisplayName")
                ? new EntityNames {
                    Name = entity.Name,
                    DisplayName = entity.DisplayName,
                    CollectionName = setting["CollectionName"],
                    CollectionDisplayName = setting["CollectionDisplayName"]
                }
                : null;
        }

        public string GetEntityNameFromCollectionName(string collectionname, bool isDisplayName) {
            var entity = _contentDefinitionManager.ListTypeDefinitions().Where(type => {
                var setting = type.Settings;
                if (isDisplayName && setting.ContainsKey("CollectionDisplayName"))
                {
                    return setting["CollectionDisplayName"] == collectionname;
                }
                if (!isDisplayName && setting.ContainsKey("CollectionName"))
                {
                    return setting["CollectionName"] == collectionname;
                }
                return false;
            });
            return entity.Count() == 1 ? entity.First().Name : null;
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