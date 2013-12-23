using Coevery.Entities.Services;
using Coevery.ContentManagement;
using Coevery.ContentManagement.FieldStorage;
using Coevery.Core.Common.Utilities;

namespace Coevery.Relationship.Fields {
    public class ReferenceField : ContentField {
        private readonly LazyField<IContent> _contentItem = new LazyField<IContent>();

        public LazyField<IContent> ContentItemField {
            get { return _contentItem; }
        }

        public object Value {
            get { return Storage.Get<object>(); }
            set { Storage.Set(value); }
        }

        public IContent ContentItem {
            get { return _contentItem.Value; }
            set { _contentItem.Value = value; }
        }
    }
}