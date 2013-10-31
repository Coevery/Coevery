using Coevery.ContentManagement.Records;

namespace Coevery.Core.Navigation.Models {
    public class MenuWidgetPartRecord : ContentPartRecord {
        public virtual int StartLevel { get; set; }
        public virtual int Levels { get; set; }
        public virtual bool Breadcrumb { get; set; }
        public virtual bool AddHomePage { get; set; }
        public virtual bool AddCurrentPage { get; set; }

        public virtual ContentItemRecord Menu { get; set; }
    }
}