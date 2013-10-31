using Coevery.ContentManagement.Records;

namespace Coevery.Core.Navigation.Models {
    public class ShapeMenuItemPartRecord : ContentPartRecord {
        /// <summary>
        /// The shape to display
        /// </summary>
        public virtual string ShapeType { get; set; }
    }
}