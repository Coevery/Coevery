using Coevery.Entities.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Orchard.Core.Common.Utilities;
using Orchard.Environment.Extensions;

namespace Coevery.Relationship.Fields {
    [CustomField]
    public class ReferenceField : ContentField {
        private readonly LazyField<IContent> _contentItem = new LazyField<IContent>();

        public LazyField<IContent> ContentItemField {
            get { return _contentItem; }
        }

        public int? Value {
            get { return Storage.Get<int?>(); }
            set { Storage.Set(value); }
        }

        public IContent ContentItem {
            get { return _contentItem.Value; }
            set { _contentItem.Value = value; }
        }
    }
}
