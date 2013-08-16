using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData;

namespace Coevery.FormDesigner.Services {
    public class LayoutManager : ILayoutManager {
        private IContentDefinitionManager _contentDefinitionManager;

        public LayoutManager(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void DeleteField(string entityName, string fieldName) {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(entityName);
            if (!contentTypeDefinition.Settings.ContainsKey("Layout")) {
                return;
            }
            var layout = contentTypeDefinition.Settings["Layout"];
            var fieldIndex = layout.IndexOf(string.Format("field-name=\"{0}\"", fieldName), StringComparison.Ordinal);
            if (fieldIndex < 0) {
                return;
            }
            var startIndex = layout.LastIndexOf("<fd-field", fieldIndex, StringComparison.Ordinal);
            var endIndex = layout.IndexOf("</fd-field>", fieldIndex, StringComparison.Ordinal) + 10;
            var newLayout = layout.Remove(startIndex, endIndex - startIndex + 1);

            contentTypeDefinition.Settings["Layout"] = newLayout;
            _contentDefinitionManager.StoreTypeDefinition(contentTypeDefinition);
        }
    }
}