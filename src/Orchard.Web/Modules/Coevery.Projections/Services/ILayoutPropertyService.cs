using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.Projections.Models;
using Orchard.Projections.ViewModels;


namespace Coevery.Projections.Services
{
    public interface ILayoutPropertyService : IDependency {
        LayoutPropertyPart CreateLayoutProperty(LayoutPropertyRecord layoutPropertyRecord);
        LayoutPropertyPart GetLayoutPropertyByQueryid(int queryid);
        void DeleteLayoutPropertyPart(int id);
    }
}