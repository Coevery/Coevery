using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Metadata.ViewModels {

    public  class  FieldTypeDto {
        public string Name { get; set; }
    }
    public class FieldViewModelDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FieldTypeDisplayName { get; set; }
        public FieldTypeDto FieldTypeName { get; set; }
        public string ParentName { get; set; }
        public List<FieldTypeDto> FieldTypes { get; set; }
    }

}
