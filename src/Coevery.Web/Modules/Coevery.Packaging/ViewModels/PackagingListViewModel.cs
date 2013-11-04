using System;
using System.Collections.Generic;
using Coevery.Packaging.Models;

namespace Coevery.Packaging.ViewModels {
    public class PackagingListViewModel {
        public DateTime? LastUpdateCheckUtc { get; set; }
        public IEnumerable<UpdatePackageEntry> Entries { get; set; }
        public dynamic Pager { get; set; }
    }
}