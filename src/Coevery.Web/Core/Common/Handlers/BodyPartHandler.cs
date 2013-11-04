using JetBrains.Annotations;
using Coevery.Core.Common.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Common.Handlers {
    [UsedImplicitly]
    public class BodyPartHandler : ContentHandler {       
        public BodyPartHandler(IRepository<BodyPartRecord> bodyRepository) {
            Filters.Add(StorageFilter.For(bodyRepository));

            OnIndexing<BodyPart>((context, bodyPart) => context.DocumentIndex
                                                                .Add("body", bodyPart.Record.Text).RemoveTags().Analyze()
                                                                .Add("format", bodyPart.Record.Format).Store());
        }
    }
}