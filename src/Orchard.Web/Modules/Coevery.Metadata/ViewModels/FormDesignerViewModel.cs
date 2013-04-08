using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Metadata.ViewModels
{
    public class FormDesignerViewModel
    {
        public ContentTypeDefinition TypeDefinition { get; set; }
        public ContentTypePartDefinition ContentPart { get; set; }

        public string GetControlType(string filedType)
        {
            switch (filedType)
            {
                case "TextField":
                    return "text";
                    
                case "BooleanField":
                    return "checkbox";
                case "NumericField":
                    return "text";
                default:
                    return "text";
            }
        }
    }
}