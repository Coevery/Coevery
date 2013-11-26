using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Coevery.ContentManagement.MetaData;

namespace Coevery.Perspectives.ViewModels {
    public class NavigationViewModel
    {
        public int PrespectiveId { get; set; }
        public int NavigationId { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string EntityName { get; set; }
        public string Title { get; set; }
        public int ParentId { get; set; }
        public string IconClass { get; set; }
        public IList<SelectListItem> Entities { get; set; }
        public IList<SelectListItem> ParentsList { get; set; }
        public string Description { get; set; }
    }
}