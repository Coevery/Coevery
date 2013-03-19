using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Metadata.ViewModels {
    public class FieldViewModelDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FieldTypeName { get; set; }
        public string ParentName { get; set; }
    }

}
