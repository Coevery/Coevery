using System.Collections.Generic;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData;

namespace Coevery.ContentManagement.Drivers {
    public interface IContentFieldDriver : IDependency {
        DriverResult BuildDisplayShape(BuildDisplayContext context);
        DriverResult BuildEditorShape(BuildEditorContext context);
        DriverResult UpdateEditorShape(UpdateEditorContext context);
        void Importing(ImportContentContext context);
        void Imported(ImportContentContext context);
        void Exporting(ExportContentContext context);
        void Exported(ExportContentContext context);
        void Describe(DescribeMembersContext context);
        IEnumerable<ContentFieldInfo> GetFieldInfo();
        void GetContentItemMetadata(GetContentItemMetadataContext context);
    }
}