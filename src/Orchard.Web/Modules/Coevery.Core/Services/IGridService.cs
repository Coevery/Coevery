using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Projections.Models;

namespace Coevery.Core.Services {
    public interface IGridService : IDependency {

        bool GenerateSortCriteria(string entityName,string sidx, string sord, int queryId);
    }
}
