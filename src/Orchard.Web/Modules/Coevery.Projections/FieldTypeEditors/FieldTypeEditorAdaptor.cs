using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {
    internal class FieldTypeEditorAdaptor : ConcreteFieldTypeEditorBase {
        private readonly IFieldTypeEditor _editor;
        public Filter Filter { get; set; }

        public FieldTypeEditorAdaptor(IFieldTypeEditor editor) {
            _editor = editor;
        }

        public override bool CanHandle(string fieldTypeName, Type storageType) {
            return false;
        }

        public override bool CanHandle(Type storageType) {
            return _editor.CanHandle(storageType);
        }

        public override string FormName {
            get { return _editor.FormName; }
        }

        public override Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return _editor.GetFilterPredicate(formState);
        }

        public override  LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return _editor.DisplayFilter(fieldName,storageName,formState);
        }

        public override Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return _editor.GetFilterRelationship(aliasName);
        }
    }
}