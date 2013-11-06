using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Core.Common.Models;
using Coevery.Data;

namespace Coevery.Core.Common.Services {
    public interface IDynamicRelationshipService : IDependency {
        void UpdateForContentItem<TContentLinkRecord>(ContentItem item, string[] links, bool isPrimary, IRepository<TContentLinkRecord> repository) where TContentLinkRecord : ContentLinkRecord, new();
        IEnumerable<IContent> GetLinks(string entityName);
    }

    public class DynamicRelationshipService : IDynamicRelationshipService {
        private readonly IContentManager _contentManager;

        public DynamicRelationshipService(
            IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void UpdateForContentItem<TContentLinkRecord>(ContentItem item, string[] links, bool isPrimary, IRepository<TContentLinkRecord> repository)
            where TContentLinkRecord : ContentLinkRecord, new() {
            var oldLinks = isPrimary
                ? repository.Fetch(r => r.PrimaryPartRecord == item.Record)
                : repository.Fetch(r => r.RelatedPartRecord == item.Record);

            var lookupNew = links != null
                ? links.ToDictionary(r => r, r => false)
                : new Dictionary<string, bool>();

            foreach (var contentRewardProgramsRecord in oldLinks) {
                var record = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.RelatedPartRecord.Id.ToString());
                if (record.Key != null) {
                    lookupNew[record.Key] = true;
                }
                else {
                    repository.Delete(contentRewardProgramsRecord);
                }
            }

            foreach (var id in lookupNew.Where(kvp => !kvp.Value).Select(kvp => int.Parse(kvp.Key))) {
                var newRecord = new TContentLinkRecord {
                    PrimaryPartRecord = isPrimary ? item.Record : _contentManager.Get(id).Record,
                    RelatedPartRecord = isPrimary ? _contentManager.Get(id).Record : item.Record
                };
                repository.Create(newRecord);
            }
        }

        public IEnumerable<IContent> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName}).List();
        }
    }
}