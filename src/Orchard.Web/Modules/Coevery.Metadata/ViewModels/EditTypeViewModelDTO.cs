using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Metadata.ViewModels {
    public class EditTypeViewDto  {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<FieldViewModelDto> Fields { get; set; }
        public bool IsEnable { get; set; }
    }

}
