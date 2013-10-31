using Coevery.Entities.Settings;

namespace Coevery.OptionSet.Settings {

    public enum ListMode {
        Dropdown,
        Radiobutton,
        Listbox,
        Checkbox
    }

    public class OptionSetFieldSettings: FieldSettings {

        /// <summary>
        /// The Option Set Id to which this field is related to
        /// </summary>
        public int OptionSetId { get; set; }

        /// <summary>
        /// Wether the user can only select leaves in the taxonomy
        /// </summary>
        public bool LeavesOnly { get; set; }

        /// <summary>
        /// Option Items
        /// </summary>
        public string Options { get; set; }

        public ListMode ListMode { get; set; }

        public OptionSetFieldSettings() {
            ListMode = ListMode.Dropdown;
        }
    }
}