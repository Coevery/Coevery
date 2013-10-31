using System.Collections.Generic;
using Coevery.Packaging.Models;

namespace Coevery.Packaging.ViewModels {
    public class PackagingSourcesViewModel {
        public IEnumerable<PackagingSource> Sources { get; set; }
    }
}