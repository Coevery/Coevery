using System;
using Coevery.Security;

namespace Coevery.ContentManagement.Aspects {
    public interface ICommonPart : IContent {
        IUser Owner { get; set; }
        IContent Container { get; set; }

        DateTime? CreatedUtc { get; set; }
        DateTime? PublishedUtc { get; set; }
        DateTime? ModifiedUtc { get; set; }

        DateTime? VersionCreatedUtc { get; set; }
        DateTime? VersionPublishedUtc { get; set; }
        DateTime? VersionModifiedUtc { get; set; }
    }
}
