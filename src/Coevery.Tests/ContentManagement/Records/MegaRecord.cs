using Coevery.Data.Conventions;

namespace Coevery.Tests.ContentManagement.Records {
    public class MegaRecord {
        public virtual int Id { get; set; }

        [StringLengthMax]
        public virtual string BigStuff { get; set; }
        public virtual string SmallStuff { get; set; }

    }
}