using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Perspectives.ViewModels {
    public class NavigationViewModel
    {
        public int PrespectiveId { get; set; }
        public int NavigationId { get; set; }
        public string EntityName { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public IList<SelectListItem> Entities { get; set; }
    }
}