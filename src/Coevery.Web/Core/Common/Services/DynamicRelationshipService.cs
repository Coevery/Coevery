using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Core.Common.Models;
using Coevery.Data;

namespace Coevery.Core.Common.Services {
    public interface IDynamicRelationshipService<TContentLinkRecord> {
        void UpdateForContentItem(ContentItem item, string[] links, bool isPrimary);
        IEnumerable<IContent> GetLinks(string entityName);
    }

    public class DynamicRelationshipService<TContentLinkRecord>
        : IDynamicRelationshipService<TContentLinkRecord>
        where TContentLinkRecord : ContentLinkRecord, new() {
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentManager _contentManager;

        public DynamicRelationshipService(
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentManager contentManager) {
            _contentLinkRepository = contentLinkRepository;
            _contentManager = contentManager;
        }

        public void UpdateForContentItem(ContentItem item, string[] links, bool isPrimary) {
            var oldLinks = isPrimary
                ? _contentLinkRepository.Fetch(r => r.PrimaryPartRecord == item.Record)
                : _contentLinkRepository.Fetch(r => r.RelatedPartRecord == item.Record);

            var lookupNew = links != null
                ? links.ToDictionary(r => r, r => false)
                : new Dictionary<string, bool>();

            foreach (var contentRewardProgramsRecord in oldLinks) {
                var record = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.RelatedPartRecord.Id.ToString());
                if (record.Key != null) {
                    lookupNew[record.Key] = true;
                }
                else {
                    _contentLinkRepository.Delete(contentRewardProgramsRecord);
                }
            }

            foreach (var id in lookupNew.Where(kvp => !kvp.Value).Select(kvp => int.Parse(kvp.Key))) {
                var newRecord = new TContentLinkRecord {
                    PrimaryPartRecord = isPrimary ? item.Record : _contentManager.Get(id).Record,
                    RelatedPartRecord = isPrimary ? _contentManager.Get(id).Record : item.Record
                };
                _contentLinkRepository.Create(newRecord);
            }
        }

        public IEnumerable<IContent> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName}).List();
        }
    }
}