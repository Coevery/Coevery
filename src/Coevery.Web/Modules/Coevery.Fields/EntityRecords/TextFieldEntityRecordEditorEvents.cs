using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;

namespace Coevery.Fields.EntityRecords {
    public class TextFieldEntityRecordEditorEvents : EntityRecordEditorEventsBase {
        public override IEnumerable<EntityRecordViewModel> FieldSettingsEditor() {
            yield return new EntityRecordViewModel {
                FieldTypeName = "TextField",
                FieldTypeDisplayName = "Text",
            };
        }

        public override void FieldSettingsEditorUpdate(string fieldType, string fieldName, SettingsDictionary settings, IUpdateModel updateModel) {
            if (fieldType != "TextField") {
                return;
            }

            settings["TextFieldSettings.IsDispalyField"] = bool.TrueString;
            settings["TextFieldSettings.Required"] = bool.TrueString;
            settings["TextFieldSettings.ReadOnly"] = bool.TrueString;
            settings["TextFieldSettings.AlwaysInLayout"] = bool.TrueString;
            settings["TextFieldSettings.IsSystemField"] = bool.TrueString;
            settings["TextFieldSettings.IsAudit"] = bool.FalseString;
        }
    }
}