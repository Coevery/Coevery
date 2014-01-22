using System.Linq;
using System.Xml.Linq;
using Coevery.Common.Extensions;
using Coevery;
using Coevery.ContentManagement.MetaData;
using Coevery.Localization;

namespace Coevery.FormDesigner.Services {
    public interface ILayoutManager : IDependency {
        void DeleteField(string typeName, string fieldName);
        void AddField(string typeName, string fieldName);
        void GenerateDefaultLayout(string typeName);
    }

    public class LayoutManager : ILayoutManager {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private const string StartTag = "<form>";
        private const string EndTag = "</form>";

        public LayoutManager(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        
        public void DeleteField(string typeName, string fieldName) {
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(typeName);
            if (typeDefinition == null
                || !typeDefinition.Settings.ContainsKey("Layout")) {
                return;
            }
            var layout = GetLayoutElement(typeDefinition.Settings["Layout"]);
            var field = layout.Descendants("fd-field").FirstOrDefault(x => x.Attribute("field-name").Value == fieldName);
            if (field == null) {
                return;
            }
            var row = field.Parent.Parent;
            field.Remove();
            if (!row.Descendants("fd-field").Any()) {
                row.Remove();
            }
            typeDefinition.Settings["Layout"] = GetLayoutString(layout);
            _contentDefinitionManager.StoreTypeDefinition(typeDefinition);
        }

        public void AddField(string typeName, string fieldName)
        {
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                return;
            }
            string layoutStr = typeDefinition.Settings.ContainsKey("Layout")
                ? typeDefinition.Settings["Layout"]
                : "<div fd-section section-columns=\"1\" section-columns-width=\"6:6\" section-title=\"" + T("General Information") + "\"><div fd-row><div fd-column></div></div></div>";
            var layout = GetLayoutElement(layoutStr);
            var emptyColumn = layout.Descendants("div").Where(n => (string)n.Attribute("fd-column") == "").FirstOrDefault(x => !x.HasElements);
            if (emptyColumn == null)
            {
                var section = layout.Descendants("div").Last(n => (string)n.Attribute("fd-section") == "");
                var row = new XElement("div", new XAttribute("fd-row", ""));
                int columnsCount = int.Parse(section.Attribute("section-columns").Value);
                for (int i = 0; i < columnsCount; i++)
                {
                    row.Add(new XElement("div", new XAttribute("fd-column", "")));
                }
                section.Add(row);
                emptyColumn = row.Elements().First();
            }
            var field = new XElement("div", new XAttribute("fd-field", ""));
            field.SetAttributeValue("field-name", fieldName);
            emptyColumn.Add(field);
            typeDefinition.Settings["Layout"] = GetLayoutString(layout);
            _contentDefinitionManager.StoreTypeDefinition(typeDefinition);
        }

        public void GenerateDefaultLayout(string typeName)
        {
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                return;
            }
            var layout = GetLayoutElement("<div fd-section section-columns=\"1\" section-columns-width=\"6:6\" section-title=\"" + T("General Information") + "\"></div>");
            var section = layout.Descendants("div").First(n => (string)n.Attribute("fd-section") == "");
            var fields = typeDefinition.Parts.First(x => x.PartDefinition.Name == typeName.ToPartName()).PartDefinition.Fields;
            foreach (var field in fields)
            {
                var row = new XElement("div", new XAttribute("fd-row", ""));
                section.Add(row);
                var column = new XElement("div", new XAttribute("fd-column", ""));
                row.Add(column);
                var fieldElement = new XElement("div", new XAttribute("fd-field", ""));
                fieldElement.SetAttributeValue("field-name", field.Name);
                column.Add(fieldElement);
            }
            typeDefinition.Settings["Layout"] = GetLayoutString(layout);
            _contentDefinitionManager.StoreTypeDefinition(typeDefinition);
        }

        private static XElement GetLayoutElement(string layoutStr) {
            layoutStr = StartTag + layoutStr + EndTag;
            return XElement.Parse(layoutStr);
        }

        private static string GetLayoutString(XElement layoutElement) {
            var layout = layoutElement.ToString();
            return layout.Substring(StartTag.Length, layout.Length - StartTag.Length - EndTag.Length);
        }
    }
}