using System.Collections.Generic;
using System.Linq;
using Coevery.Caching;
using Coevery.Common.Extensions;
using Coevery.Data;
using Coevery.Entities.Models;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Entities.Extensions {
    public class ContentDefinitionExtension : IContentDefinitionExtension {
        private const string ContentDefinitionSignal = "ContentDefinitionManager";
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRepository<EntityMetadataRecord> _entityMetadataRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public ContentDefinitionExtension(
            IContentDefinitionManager contentDefinitionManager,
            IRepository<EntityMetadataRecord> entityMetadataRepository,
            ICacheManager cacheManager,
            ISignals signals) {
            _contentDefinitionManager = contentDefinitionManager;
            _entityMetadataRepository = entityMetadataRepository;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions() {
            return _cacheManager.Get("UserContentTypeDefinitions", ctx => {
                MonitorContentDefinitionSignal(ctx);

                var metaEntities = _entityMetadataRepository.Table.Select(x => x.Name).Distinct().ToList();
                if (metaEntities.Count == 0) {
                    return Enumerable.Empty<ContentTypeDefinition>();
                }

                return (from type in _contentDefinitionManager.ListTypeDefinitions()
                    where metaEntities.Contains(type.Name)
                    select type).ToList();
            });
        }

        public IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions() {
            return _cacheManager.Get("UserContentPartDefinitions", ctx => {
                MonitorContentDefinitionSignal(ctx);

                var metaEntities = _entityMetadataRepository.Table.Select(x => x.Name).Distinct().ToList();
                if (metaEntities.Count == 0) {
                    return Enumerable.Empty<ContentPartDefinition>();
                }

                return (from part in _contentDefinitionManager.ListPartDefinitions()
                    where metaEntities.Contains(part.Name.RemovePartSuffix())
                    select part).ToList();
            });
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
                if (isDisplayName && setting.ContainsKey("CollectionDisplayName")) {
                    return setting["CollectionDisplayName"] == collectionname;
                }
                if (!isDisplayName && setting.ContainsKey("CollectionName")) {
                    return setting["CollectionName"] == collectionname;
                }
                return false;
            });
            return entity.Count() == 1 ? entity.First().Name : null;
        }

        private void MonitorContentDefinitionSignal(AcquireContext<string> ctx) {
            ctx.Monitor(_signals.When(ContentDefinitionSignal));
        }
    }
}