using System.ComponentModel.DataAnnotations;
using Coevery.DisplayManagement.Shapes;

namespace Coevery.Core.Common.OwnerEditor {
    public class OwnerEditorViewModel : Shape {
        [Required]
        public string Owner { get; set; }
    }
}