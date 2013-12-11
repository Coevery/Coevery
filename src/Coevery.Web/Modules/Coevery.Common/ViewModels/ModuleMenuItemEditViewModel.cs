using System.Web.Mvc;
using Coevery.Common.Models;
using System.Collections.Generic;

namespace Coevery.Common.ViewModels {
    public class ModuleMenuItemEditViewModel {
        public string EntityName { get; set; }
        public string IconClass { get; set; }
        public IEnumerable<SelectListItem> Entities { get; set; }
    }
}