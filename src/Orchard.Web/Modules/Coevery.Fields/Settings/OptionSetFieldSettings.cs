using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Coevery.Fields.Settings {
    public enum SelectionMode {
        Checkbox,
        Radiobutton,
        DropDown
    }
    public enum DependentType {
        None,
        Control,
        Dependent,
        ControlAndDependent
    }

    public class OptionSetFieldSettings : FieldSettings {
        public static readonly string[] LabelSeperator = new string[] { ";", "\r\n" };

        public int DisplayLines { get; set; }
        public SelectionMode DisplayOption { get; set; }
        public int SelectCount { get; set; }

        //Dependency related
        public DependentType DependencyMode { get; set; }
        public int OptionSetId { get; set; } 

        //Only used when creating
        public string LabelsStr { get; set; }
        public int DefaultValue { get; set; }

        public OptionSetFieldSettings() {
            DisplayOption = SelectionMode.DropDown;
            DefaultValue = 0;
            SelectCount = 1;
            DisplayLines = 1;
            DependencyMode = DependentType.None;
        }
    }
}
