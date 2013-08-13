using System.Collections.Generic;
using System.Web.Mvc;
using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings
{
    public class ReferenceFieldSettings : FieldSettings
    {
        public bool DisplayAsLink { get; set; }

        /// <summary>
        /// The ID of the Query object used to generate the list of selectable content items.
        /// </summary>
        public string ContentTypeName { get; set; }

        public int QueryId { get; set; }

        public IList<SelectListItem> ContentTypeList { get; set; }
    }
}
