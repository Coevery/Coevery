using Coevery.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;

namespace Coevery.Autoroute.Models {
    public class AutoroutePartRecord : ContentPartVersionRecord {

        public virtual bool UseCustomPattern { get; set; }
        
        [StringLength(2048)]
        public virtual string CustomPattern { get; set; }
        
        [StringLength(2048)]
        public virtual string DisplayAlias { get; set; }
    }
}
