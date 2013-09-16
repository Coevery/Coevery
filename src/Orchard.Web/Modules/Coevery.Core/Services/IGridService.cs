using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Coevery.Core.Services {
    public interface IGridService:IDependency {
        object GetPageRows(int page, int rows, IEnumerable<object> totalRecords);
    }
}
