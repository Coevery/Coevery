using System.Web.Mvc;
using Coevery.Common.Models;
using System.Collections.Generic;

namespace Coevery.Common.ViewModels {
    public class ModuleMenuItemEditViewModel {
        public int ContenTypeId { get; set; }
        public ModuleMenuItemPart Part { get; set; }
        public IEnumerable<SelectListItem> ContentTypes { get; set; }
    }
}