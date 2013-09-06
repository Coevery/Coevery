using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class PageContext
    {
        public IContent Page { get; private set; }
        public string Group { get; set; }

        public PageContext(IContent page, string group)
        {
            Page = page;
            Group = group;
        }
    }
}
