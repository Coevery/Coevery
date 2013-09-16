using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.Logging;

namespace Coevery.Core.Services {
    public class GridService:IGridService {
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public GridService() {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public object GetPageRows(int page, int rows, IEnumerable<object> totalRecords ) {
            var length = Math.Min(rows, totalRecords.Count());
            var result = new object[length];
            for (var index = 0; index < length; index++) {
                result[index] = totalRecords.ElementAtOrDefault(index + (page - 1) * rows);
            }
            return result;
        }

    }
}