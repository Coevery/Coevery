using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Core.Containers.Models;

namespace Coevery.Core.Containers.Handlers {
    public class ContainablePartHandler : ContentHandler {
        public ContainablePartHandler(IRepository<ContainablePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}