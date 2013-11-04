using System.ComponentModel.DataAnnotations;

namespace Coevery.Packaging.ViewModels {
    public class PackagingAddSourceViewModel {

        [Required]
        public string Url { get; set; }
    }
}