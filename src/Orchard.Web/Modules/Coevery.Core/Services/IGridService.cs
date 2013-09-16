using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Coevery.Core.Services {
    public interface IGridService : IDependency {
        IEnumerable<TRow> GetPagedRows<TRow>(int page, int rows, IEnumerable<TRow> totalRecords);
        IEnumerable<TRow> GetSortedRows<TRow>(string sidx, string sord, IEnumerable<TRow> rawRecords);
    }
}
