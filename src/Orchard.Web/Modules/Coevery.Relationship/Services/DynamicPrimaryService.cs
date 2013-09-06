using System.Collections.Generic;
using System.Linq;
using Coevery.Relationship.Models;
using Coevery.Relationship.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Coevery.Relationship.Services {
    public interface IDynamicPrimaryService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> {
        void UpdateForContentItem(ContentItem item, string[] links);
        IEnumerable<ContentPartRecord> GetLinks(string entityName);
    }

    public class DynamicPrimaryService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        : IDynamicPrimaryService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        where TPrimaryPart : ContentPart<TPrimaryPartRecord>
        where TRelatedPart : ContentPart<TRelatedPartRecord>
        where TPrimaryPartRecord : ContentPartRecord
        where TRelatedPartRecord : ContentPartRecord
        where TContentLinkRecord : ContentLinkRecord<TPrimaryPartRecord, TRelatedPartRecord>, new() {
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentManager _contentManager;

        public DynamicPrimaryService(
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentManager contentManager) {
            _contentLinkRepository = contentLinkRepository;
            _contentManager = contentManager;
        }

        public void UpdateForContentItem(ContentItem item, string[] links) {
            var record = item.As<TPrimaryPart>().Record;
            var oldLinks = _contentLinkRepository.Fetch(
                r => r.PrimaryPartRecord == record);
            var lookupNew = links != null
                ? links.ToDictionary(r => r, r => false)
                : new Dictionary<string, bool>();
            // Delete the rewards that are no longer there and mark the ones that should stay
            foreach (var contentRewardProgramsRecord in oldLinks) {
                var newReward = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.RelatedPartRecord.Id.ToString());
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
                    PrimaryPartRecord = record,
                    RelatedPartRecord = _contentManager.Get(int.Parse(reward)).As<TRelatedPart>().Record
                });
            }
        }

        public IEnumerable<ContentPartRecord> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName})
                .List().AsPart<TRelatedPart>().Select(x => x.Record);
        }
    }
}