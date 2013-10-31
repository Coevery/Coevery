using JetBrains.Annotations;
using Coevery.Core.Common.Models;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Common.DateEditor {
    [UsedImplicitly]
    public class DateEditorHandler : ContentHandler {
        public DateEditorHandler() {
            OnPublished<CommonPart>((context, part) => {
                var settings = part.TypePartDefinition.Settings.GetModel<DateEditorSettings>();
                if (!settings.ShowDateEditor) {
                    return;
                }

                var thisIsTheInitialVersionRecord = part.ContentItem.Version < 2;
                var theDatesHaveNotBeenModified = part.CreatedUtc == part.VersionCreatedUtc;
                var theContentDateShouldBeUpdated = thisIsTheInitialVersionRecord && theDatesHaveNotBeenModified;

                if (theContentDateShouldBeUpdated) {
                    // "touch" CreatedUtc in ContentItemRecord
                    part.CreatedUtc = part.PublishedUtc;
                }
            });
        }
    }
}
