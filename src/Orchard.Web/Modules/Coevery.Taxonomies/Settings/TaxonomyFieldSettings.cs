using System.Collections.Generic;
using Coevery.Taxonomies.Models;
using Coevery.Entities.Settings;

namespace Coevery.Taxonomies.Settings {

    public enum ListMode {
        Dropdown,
        Radiobutton,
        Listbox,
        Checkbox
    }

    public class TaxonomyFieldSettings: FieldSettings {

        /// <summary>
        /// The Taxonomy'Id to which this field is related to
        /// </summary>
        public int TaxonomyId { get; set; }

        /// <summary>
        /// Wether the user can only select leaves in the taxonomy
        /// </summary>
        public bool LeavesOnly { get; set; }

        /// <summary>
        /// Wether the user can select only one term or not
        /// </summary>
        public bool SingleChoice { get; set; }

        /// <summary>
        /// Option Items
        /// </summary>
        public string Options { get; set; }

        public ListMode ListMode { get; set; }

        public TaxonomyFieldSettings() {
            ListMode = ListMode.Dropdown;
        }
    }
}