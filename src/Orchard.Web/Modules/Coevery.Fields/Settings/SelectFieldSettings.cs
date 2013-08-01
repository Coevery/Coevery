using System;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Coevery.Fields.Settings {
    public enum SelectionMode {
        Checkbox,
        Radiobutton,
        DropDown
    }

    public class SelectFieldSettings : FieldSettings {
        public static readonly string[] LabelSeperator = new string[] { ";", "\r\n" };

        public int DisplayLines { get; set; }
        public SelectionMode DisplayOption { get; set; }
        public int SelectCount { get; set; }

        //Dependency related
        public bool HasDependency { get; set; }
        public int FieldSettingId { get; set; }

        //Only used when creating
        public string LabelsStr { get; set; }
        public int DefaultValue { get; set; }
        
        public SelectFieldSettings() {
            DisplayOption = SelectionMode.DropDown;
            DefaultValue = 0;
            SelectCount = 1;
            DisplayLines = 1;
        }

        public bool CheckValid(IUpdateModel updateModel, Localizer t, int itemCount, bool isCreating) {            
            if (itemCount <= 0) {
                updateModel.AddModelError("SelectSettings", t("No valid label exists."));
                return false;
            }
            //Only check when creating
            if (isCreating && (DefaultValue > itemCount || DefaultValue < 0)) {
                updateModel.AddModelError("SelectSettings", t("DefaultValue is out of range."));
                return false;
            }

            //Setting value range
            if (SelectCount < 1 || SelectCount > itemCount) {
                updateModel.AddModelError("SelectSettings", t("SelectCount is out of range."));
                return false;
            }
            if (DisplayLines < 1 || DisplayLines > itemCount) {
                updateModel.AddModelError("SelectSettings", t("DisplayLines is out of range."));
                return false;
            }

            //Display option and select count match
            if ((DisplayOption == SelectionMode.Checkbox && SelectCount == 1)
                || (DisplayOption == SelectionMode.Radiobutton && SelectCount > 1)) {
                updateModel.AddModelError("SelectSettings", t("The display option and select count don't match."));
                return false;
            }
            return true;
        }
    }
}
