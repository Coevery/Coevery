using System.Collections.Generic;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData;

namespace Coevery.ContentManagement.Drivers {
    public interface IContentPartDriver : IDependency {
        DriverResult BuildDisplay(BuildDisplayContext context);
        DriverResult BuildEditor(BuildEditorContext context);
        DriverResult UpdateEditor(UpdateEditorContext context);
        void Importing(ImportContentContext context);
        void Imported(ImportContentContext context);
        void Exporting(ExportContentContext context);
        void Exported(ExportContentContext context);
        IEnumerable<ContentPartInfo> GetPartInfo();
        void GetContentItemMetadata(GetContentItemMetadataContext context);
    }
}