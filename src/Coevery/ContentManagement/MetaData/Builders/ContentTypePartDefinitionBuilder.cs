using System.Linq;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement.MetaData.Builders {
    public abstract class ContentTypePartDefinitionBuilder {
        protected readonly SettingsDictionary _settings;

        protected ContentTypePartDefinitionBuilder(ContentTypePartDefinition part) {
            Name = part.PartDefinition.Name;
            _settings = new SettingsDictionary(part.Settings.ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        public string Name { get; private set; }

        public ContentTypePartDefinitionBuilder WithSetting(string name, string value) {
            _settings[name] = value;
            return this;
        }
    }
}
