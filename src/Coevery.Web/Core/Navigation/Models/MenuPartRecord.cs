using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement.Records;

namespace Coevery.Core.Navigation.Models {
    public class MenuPartRecord : ContentPartRecord {
        public const ushort DefaultMenuTextLength = 255;

        [StringLength(DefaultMenuTextLength)]
        public virtual string MenuText { get; set; }
        public virtual string MenuPosition { get; set; }
        public virtual int MenuId { get; set; }
    }
}