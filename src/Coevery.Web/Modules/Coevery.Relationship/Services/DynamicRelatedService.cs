using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.Relationship.Models;

namespace Coevery.Relationship.Services {
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
            // Delete the rewards that are no longer there and mark the ones that should stay
            foreach (var contentRewardProgramsRecord in oldLinks) {
                var newReward = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.PrimaryPartRecord.Id.ToString());
                if (newReward.Key != null) {
                    lookupNew[newReward.Key] = true;
                }
                else {
                    _contentLinkRepository.Delete(contentRewardProgramsRecord);
                }
            }
            // Add the new rewards
            foreach (var reward in lookupNew.Where(kvp => !kvp.Value).Select(kvp => kvp.Key)) {
                _contentLinkRepository.Create(new TContentLinkRecord {
                    PrimaryPartRecord = _contentManager.Get(int.Parse(reward)).Record,
                    RelatedPartRecord = item.Record
                });
            }
        }

        public IEnumerable<IContent> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName}).List();
        }
    }
}