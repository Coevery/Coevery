using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Coevery.Entities.ViewModels;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Entities.ViewModels {
    public class AddFieldViewModel {
        public AddFieldViewModel() {
            Fields = new List<TemplateViewModel>();
        }

        /// <summary>
        /// The technical name of the field
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The display name of the field
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// The selected field type
        /// </summary>
        [Required]
        public string FieldTypeName { get; set; }

        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public bool AlwaysInLayout { get; set; }
        public bool IsAudit { get; set; }
        public string HelpText { get; set; }
        public bool AddInLayout { get; set; }

        /// <summary>
        /// The part to add the field to
        /// </summary>
        public EditPartViewModel Part { get; set; }

        /// <summary>
        /// List of the available Field types
        /// </summary>
        public IEnumerable<TemplateViewModel> Fields { get; set; }

        public IEnumerable<TemplateViewModel> TypeTemplates { get; set; }
    }
}