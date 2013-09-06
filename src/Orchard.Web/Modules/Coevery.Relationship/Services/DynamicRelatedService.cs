using System.Collections.Generic;
using System.Linq;
using Coevery.Relationship.Models;
using Coevery.Relationship.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Coevery.Relationship.Services {
    public interface IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> {
        void UpdateForContentItem(ContentItem item, string[] links);
        IEnumerable<ContentPartRecord> GetLinks(string entityName);
    }

    public class DynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        : IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        where TPrimaryPart : ContentPart<TPrimaryPartRecord>
        where TRelatedPart : ContentPart<TRelatedPartRecord>
        where TPrimaryPartRecord : ContentPartRecord
        where TRelatedPartRecord : ContentPartRecord
        where TContentLinkRecord : ContentLinkRecord<TPrimaryPartRecord, TRelatedPartRecord>, new() {
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentManager _contentManager;

        public DynamicRelatedService(
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentManager contentManager) {
            _contentLinkRepository = contentLinkRepository;
            _contentManager = contentManager;
        }

        public void UpdateForContentItem(ContentItem item, string[] links) {
            var record = item.As<TRelatedPart>().Record;
            var oldLinks = _contentLinkRepository.Fetch(
                r => r.RelatedPartRecord == record);
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
                    PrimaryPartRecord = _contentManager.Get(int.Parse(reward)).As<TPrimaryPart>().Record,
                    RelatedPartRecord = record
                });
            }
        }

        public IEnumerable<ContentPartRecord> GetLinks(string entityName) {
            return _contentManager.Query(VersionOptions.Published, new[] {entityName})
                .List().AsPart<TPrimaryPart>().Select(x => x.Record);
        }
    }
}