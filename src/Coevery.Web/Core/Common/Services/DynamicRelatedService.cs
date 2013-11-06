using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Core.Common.Models;
using Coevery.Data;

namespace Coevery.Core.Common.Services {
    public interface IDynamicRelatedService<TContentLinkRecord> {
        void UpdateForContentItem(ContentItem item, string[] links);
        IEnumerable<IContent> GetLinks(string entityName);
    }

    public class DynamicRelatedService<TContentLinkRecord>
        : IDynamicRelatedService<TContentLinkRecord>
        where TContentLinkRecord : ContentLinkRecord, new() {
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentManager _contentManager;

        public DynamicRelatedService(
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentManager contentManager) {
            _contentLinkRepository = contentLinkRepository;
            _contentManager = contentManager;
        }

        public void UpdateForContentItem(ContentItem item, string[] links) {
            var oldLinks = _contentLinkRepository.Fetch(
                r => r.RelatedPartRecord == item.Record);
            var lookupNew = links != null
                ? links.ToDictionary(r => r, r => false)
                : new Dictionary<string, bool>();

            foreach (var contentRewardProgramsRecord in oldLinks) {
                var record = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.PrimaryPartRecord.Id.ToString());
                if (record.Key != null) {
                    lookupNew[record.Key] = true;
                }
                else {
                    _contentLinkRepository.Delete(contentRewardProgramsRecord);
                }
            }

            foreach (var id in lookupNew.Where(kvp => !kvp.Value).Select(kvp => kvp.Key)) {
                _contentLinkRepository.Create(new TContentLinkRecord {
                    PrimaryPartRecord = _contentManager.Get(int.Parse(id)).Record,
                    RelatedPartRecord = item.Record
                });

            }
        }

        public IEnumerable<IContent> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName}).List();
        }
    }
}