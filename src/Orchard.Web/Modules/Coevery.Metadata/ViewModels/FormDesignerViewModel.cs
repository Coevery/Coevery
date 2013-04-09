using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Metadata.HtmlControlProviders;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Metadata.ViewModels
{
    public class FormDesignerViewModel
    {
        public ContentTypeDefinition TypeDefinition { get; set; }
        public ContentTypePartDefinition ContentPart { get; set; }
        public IDictionary<string, string> HtmlControlDescs { get; set; }
    }
}