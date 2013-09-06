using System;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Utilities;
using Orchard.ContentManagement;
using Orchard.Security;

namespace Coevery.Core.Models {
    public class CoeveryCommonPart : ContentPart<CoeveryCommonPartRecord>, ICommonPart {
        private readonly LazyField<IUser> _owner = new LazyField<IUser>();
        private readonly LazyField<IUser> _modifier = new LazyField<IUser>();
        private readonly LazyField<IContent> _container = new LazyField<IContent>();

        public LazyField<IUser>  OwnerField {
            get { return _owner; }
        }

        public LazyField<IUser> ModifierField {
            get { return _modifier; }
        }

        public LazyField<IContent> ContainerField {
            get { return _container; }
        }

        public IUser Owner {
            get { return _owner.Value; }
            set { _owner.Value = value; }
        }

        public IUser Modifer {
            get { return _modifier.Value; }
            set { _modifier.Value = value; }
        }

        public IContent Container {
            get { return _container.Value; }
            set { _container.Value = value; }
        }

        public DateTime? CreatedUtc {
            get { return Record.CreatedUtc; }
            set { Record.CreatedUtc = value; }
        }

        public DateTime? ModifiedUtc {
            get { return Record.ModifiedUtc; }
            set { Record.ModifiedUtc = value; }
        }

        private CoeveryCommonPartVersionRecord PartVersionRecord {
            get {
                var versionPart = this.As<ContentPart<CoeveryCommonPartVersionRecord>>();
                return versionPart == null ? null : versionPart.Record;
            }
        }

        public DateTime? VersionCreatedUtc {
            get { return PartVersionRecord == null ? CreatedUtc : PartVersionRecord.CreatedUtc; }
            set {
                if (PartVersionRecord != null)
                    PartVersionRecord.CreatedUtc = value;
            }
        }

        public DateTime? VersionModifiedUtc {
            get { return PartVersionRecord == null ? ModifiedUtc : PartVersionRecord.ModifiedUtc; }
            set {
                if (PartVersionRecord != null)
                    PartVersionRecord.ModifiedUtc = value;
            }
        }

        //Rely on version functionality
        public DateTime? PublishedUtc { get; set; }
        public DateTime? VersionPublishedUtc { get; set; }
    }
}
