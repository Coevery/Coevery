using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Core.Contents.Settings;
using Orchard.Data;

namespace Coevery.Metadata.Drivers {
    public abstract class DynamicContentsHandler<TRecord> : ContentHandler where TRecord : ContentPartRecord, new() {
        public DynamicContentsHandler(IRepository<TRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }


}