using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.MetaData.Services;
using Coevery.Entities.Services;

namespace Coevery.Entities.Models {
    public class EntityMetadataPart : ContentPart<EntityMetadataRecord> {
        private readonly ISettingService _settingService = new SettingService(new SettingsFormatter());

        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string DisplayName {
            get { return Record.DisplayName; }
            set { Record.DisplayName = value; }
        }

        public SettingsDictionary EntitySetting {
            get { return _settingService.ParseSetting(Record.Settings); }
            set { Record.Settings = _settingService.CompileSetting(value); }
        }

        public IList<FieldMetadataRecord> FieldMetadataRecords {
            get { return Record.FieldMetadataRecords; }
        }
    }
}