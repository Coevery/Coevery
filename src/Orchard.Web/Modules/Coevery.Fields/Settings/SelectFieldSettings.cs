namespace Coevery.Fields.Settings {
    public enum SelectionMode {
        Checkbox,
        Radiobutton,
        DropDown
    }

    public class SelectFieldSettings : FieldSettings {
        public static readonly string[] LabelSeperator = new string[] { ";","\n\r" };
        public int DisplayLines { get; set; }
        public SelectionMode DisplayOption { get; set; }
        public int SelectCount { get; set; }
        public string LabelsStr { get; set; }

        //DefaultValue start from 0, but matches item with index DefaultValue-1
        public int DefaultValue { get; set; }

        public SelectFieldSettings() {
            DisplayOption = SelectionMode.DropDown;
            DefaultValue = 0;
            SelectCount = 1;
            DisplayLines = 4;
        }
    }
}
