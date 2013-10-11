using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Projections.Models;
using Orchard.Projections.Services;

namespace Coevery.Core.Services {
    public class GridService : IGridService {
        private const string Format = @"<Form><Description>{0}</Description><Sort>{1}</Sort></Form>";
        private readonly IQueryService _queryService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public GridService(
            IQueryService queryService) {
            _queryService = queryService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public bool GenerateSortCriteria(string entityName, string sortColumns, string lastOrder, int queryId) {
            if (string.IsNullOrWhiteSpace(entityName)) {
                return false;
            }
            if (string.IsNullOrWhiteSpace(sortColumns)) {
                return true;
            }
            var query = _queryService.GetQuery(queryId);
            query.SortCriteria.Clear();
            var index = 0;
            foreach (var sortColumn in (sortColumns + " " + lastOrder).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                var sortInfo = sortColumn.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                query.SortCriteria.Add(new SortCriterionRecord {
                    Category = entityName + "ContentFields",
                    Type = entityName + "." + sortInfo.First() + ".",
                    State = FormParametersHelper.ToString(new Dictionary<string,string> {
                        { "Sort", (sortInfo.Last() == "asc").ToString().ToLower()}
                    }),
                    Description = sortInfo.First(),
                    Position = ++index
                });
            }
            return true;
        }
    }
}

/*Abandoned code
try {
                if (typeof (JObject) == typeof (TRow)) {
                    if (orderColumns == "asc") {
                        return rawRecords.OrderBy(row => (row as JObject)[sortColumns]);
                    } else if (orderColumns == "desc") {
                        return rawRecords.OrderByDescending(row => (row as JObject)[sortColumns]);
                    }
                    return null;
                }

                if (orderColumns == "asc") {
                    return rawRecords.OrderBy(row => row.GetType().GetProperty(sortColumns).GetValue(row, null));
                } else if (orderColumns == "desc") {
                    return rawRecords.OrderByDescending(row => row.GetType().GetProperty(sortColumns).GetValue(row, null));
                }
            }
            catch (Exception ex) {
                Logger.Log(LogLevel.Error, ex, "The column name is invalid property for the row model.");
            }
            finally {
                if (string.IsNullOrWhiteSpace(sortColumns) || string.IsNullOrWhiteSpace(orderColumns))
                    Logger.Log(LogLevel.Error, null, "Sort rows for grid failed!");
            }
            return null;
 */