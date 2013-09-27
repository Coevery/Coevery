using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {
    internal class FieldTypeEditorAdaptor: IConcreteFieldTypeEditor {
        private readonly IFieldTypeEditor _editor;
        public Filter Filter { get; set; }

        public FieldTypeEditorAdaptor(IFieldTypeEditor editor) {
            _editor = editor;
        }

        public bool CanHandle(string fieldTypeName, Type storageType) {
            return _editor.CanHandle(storageType);
        }

        public bool CanHandle(Type storageType) {
            return _editor.CanHandle(storageType);
        }

        public string FormName {
            get { return _editor.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return _editor.GetFilterPredicate(formState);
        }

        public Orchard.Localization.LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return _editor.DisplayFilter(fieldName,storageName,formState);
        }

        public Action<Orchard.ContentManagement.IAliasFactory> GetFilterRelationship(string aliasName) {
            return _editor.GetFilterRelationship(aliasName);
        }
    }
}