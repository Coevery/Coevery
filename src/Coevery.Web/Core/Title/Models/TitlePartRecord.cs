using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement.Records;

namespace Coevery.Core.Title.Models {
    public class TitlePartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string Title { get; set; }
    }
}
