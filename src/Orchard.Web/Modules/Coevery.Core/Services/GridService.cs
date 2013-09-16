using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.Logging;

namespace Coevery.Core.Services {
    public class GridService : IGridService {
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public GridService() {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public IEnumerable<TRow> GetPagedRows<TRow>(int page, int rows, IEnumerable<TRow> totalRecords) {
            var length = Math.Min(rows, totalRecords.Count());
            var result = new TRow[length];
            for (var index = 0; index < length; index++) {
                result[index] = totalRecords.ElementAtOrDefault(index + (page - 1) * rows);
            }
            return result;
        }

        public IEnumerable<TRow> GetSortedRows<TRow>(string sidx, string sord, IEnumerable<TRow> rawRecords) {
            if (string.IsNullOrWhiteSpace(sidx)) {
                return rawRecords;
            }
            try {
                if (sord == "asc") {
                    return rawRecords.OrderBy(row => row.GetType().GetProperty(sidx).GetValue(row, null));
                } else if (sord == "desc") {
                    return rawRecords.OrderByDescending(row => row.GetType().GetProperty(sidx).GetValue(row, null));
                }
            }
            catch (Exception ex) {
                Logger.Log(LogLevel.Error, ex, "The column name is invalid property for the row model.");
            }
            finally {
                if (string.IsNullOrWhiteSpace(sidx) || string.IsNullOrWhiteSpace(sord))
                    Logger.Log(LogLevel.Error, null, "Sort rows for grid failed!");
            }
            return null;
        }
    }
}