using System.Collections.Generic;
using System.Linq;
using Coevery.Relationship.Models;
using Coevery.Relationship.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Coevery.Relationship.Services {
    public interface IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> : IDependency {
        void UpdateForContentItem(ContentItem item, IEnumerable<RelationshipEntry> links);
        IEnumerable<ContentPartRecord> GetLinks();
    }

    public abstract class DynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        : IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        where TPrimaryPart : ContentPart<TPrimaryPartRecord>
        where TRelatedPart : ContentPart<TRelatedPartRecord>
        where TPrimaryPartRecord : ContentPartRecord
        where TRelatedPartRecord : ContentPartRecord
        where TContentLinkRecord : IContentLinkRecord, new() {
        private readonly IRepository<TPrimaryPartRecord> _primaryRepository;
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentManager _contentManager;

        protected DynamicRelatedService(
            IRepository<TPrimaryPartRecord> primaryRepository,
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentManager contentManager) {

            _primaryRepository = primaryRepository;
            _contentLinkRepository = contentLinkRepository;
            _contentManager = contentManager;
        }

        public void UpdateForContentItem(ContentItem item, IEnumerable<RelationshipEntry> links) {
            var record = item.As<TRelatedPart>().Record;
            var oldLinks = _contentLinkRepository.Fetch(
                r => r.RelatedPartRecord == record);
            var lookupNew = links
                .Where(e => e.IsChecked)
                .Select(e => e.Id)
                .ToDictionary(r => r, r => false);
            // Delete the rewards that are no longer there and mark the ones that should stay
            foreach (var contentRewardProgramsRecord in oldLinks) {
                var newReward = lookupNew.FirstOrDefault(x => x.Key == contentRewardProgramsRecord.PrimaryPartRecord.Id);
                if (newReward.Key != 0) {
                    lookupNew[newReward.Key] = true;
                }
                else {
                    _contentLinkRepository.Delete(contentRewardProgramsRecord);
                }
            }
            // Add the new rewards
            foreach (var reward in lookupNew.Where(kvp => !kvp.Value).Select(kvp => kvp.Key)) {
                _contentLinkRepository.Create(new TContentLinkRecord {
                    PrimaryPartRecord = _contentManager.Get(reward).As<TPrimaryPart>().Record,
                    RelatedPartRecord = record
                });
            }
        }

        public IEnumerable<ContentPartRecord> GetLinks() {
            return _primaryRepository.Table.ToList();
        }
    }
}