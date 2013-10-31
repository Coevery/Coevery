using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Aspects;

namespace Coevery.Core.Title.Models {
    public class TitlePart : ContentPart<TitlePartRecord>, ITitleAspect {
        [Required]
        public string Title {
            get { return Record.Title; }
            set { Record.Title = value; }
        }
    }
}