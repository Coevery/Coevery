using Coevery.Data.Conventions;

namespace Coevery.Tests.Records {
    public class BigRecord {
        public virtual int Id { get; set; }
        [StringLengthMax]
        public virtual string Body { get; set; }

        [StringLengthMax]
        public virtual byte[] Banner { get; set; }
    }
}