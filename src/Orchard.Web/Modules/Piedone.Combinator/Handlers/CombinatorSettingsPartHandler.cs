using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.Combinator.Models;
using Piedone.Combinator.Services;

namespace Piedone.Combinator
{
    [OrchardFeature("Piedone.Combinator")]
    public class CombinatorSettingsPartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public CombinatorSettingsPartHandler(
            IRepository<CombinatorSettingsPartRecord> repository,
            ICacheFileService cacheFileService)
        {
            Filters.Add(new ActivatingFilter<CombinatorSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;

            OnLoaded<CombinatorSettingsPart>((context, part) =>
            {
                // Add loaders that will load content just-in-time
                part.CacheFileCountField.Loader(() => cacheFileService.GetCount());
            });
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            var groupInfo = new GroupInfo(T("Combinator")); // Addig a new group to the "Settings" menu.
            groupInfo.Id = "Combinator";
            context.Metadata.EditorGroupInfo.Add(groupInfo);
        }
    }
}