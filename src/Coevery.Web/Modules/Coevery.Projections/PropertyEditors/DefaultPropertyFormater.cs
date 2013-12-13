using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.ContentManagement;
using Coevery.DisplayManagement;
using Coevery.Entities.Services;
using Coevery.Logging;
using Coevery.Projections.ModelBinding;

namespace Coevery.Projections.PropertyEditors {
    public class DefaultPropertyFormater : IPropertyFormater {
        private readonly IShapeFactory _shapeFactory;
        private readonly IEnumerable<IContentFieldFormatProvider> _contentFieldFormatProviders;
        private readonly IEnumerable<IPropertyEditor> _propertyEditors;

        public DefaultPropertyFormater(
            IShapeFactory shapeFactory,
            IEnumerable<IPropertyEditor> propertyEditors, 
            IEnumerable<IContentFieldFormatProvider> contentFieldFormatProviders) {
            _shapeFactory = shapeFactory;
            _propertyEditors = propertyEditors;
            _contentFieldFormatProviders = contentFieldFormatProviders;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string GetForm(Type type) {
            var propertyEditor = GetPropertyEditor(type);
            if(propertyEditor == null) {
                return null;
            }

            return propertyEditor.FormName;
        }

        public dynamic Format(Type type, object value, dynamic formState) {
            return Format(null, type, value, formState);
        }

        public dynamic Format(ContentField field, Type type, object value, dynamic formState) {
            var propertyEditor = GetPropertyEditor(type);

            if (propertyEditor == null) {
                var stringValue = Convert.ToString(value);

                if (String.IsNullOrEmpty(stringValue)) {
                    return String.Empty;
                }

                return new HtmlString(stringValue);
            }
            if (field != null) {
                _contentFieldFormatProviders.Invoke(p => { p.SetFormat(field, formState); }, Logger);
            }
            return propertyEditor.Format(_shapeFactory, value, formState);
        }

        private IPropertyEditor GetPropertyEditor(Type type) {
            return _propertyEditors.FirstOrDefault(x => x.CanHandle(type));
        }
    }
}