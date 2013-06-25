using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data.Conventions;
using Orchard.Projections.Models;

namespace Coevery.Core.Models
{
    public class ViewPartRecord
    {
        public virtual int Id { get; set; }

        public virtual int ContentTypeDefinitionRecord_id { get; set; }
        public virtual int ProjectionPartRecord_id { get; set; }

        //[Aggregate]
        //public virtual ContentTypeDefinitionRecord ContentTypeDefinitionRecord { get; set; }
        //[Aggregate]
        //public virtual ProjectionPartRecord ProjectionPartRecord { get; set; }
    }
}