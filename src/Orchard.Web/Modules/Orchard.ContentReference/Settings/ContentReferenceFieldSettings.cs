using System.Collections.Generic;
using System.Web.Mvc;

namespace Contrib.ContentReference.Settings {
    public class ContentReferenceFieldSettings {
        public bool DisplayAsLink { get; set; }
        public bool Required { get; set; }

        /// <summary>
        /// The ID of the Query object used to generate the list of selectable content items.
        /// </summary>
        public int QueryId { get; set; }

        public IList<SelectListItem> QueryList { get; set; }
    }
}
