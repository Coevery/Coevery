using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Common.Extensions;
using Coevery.Common.Services;
using Coevery.Common.ViewModels;
using Coevery.Logging;
using Coevery.Projections.Services;
using Newtonsoft.Json.Linq;
using NHibernate.Linq;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Models;
using System.Linq;
using Coevery.Tokens;
using Coevery.UI.Navigation;

namespace Coevery.Projections.Controllers {

    public class QueryExecuteContext {
        public IEnumerable<Action> Executing { get; set; }
        public IEnumerable<Action> Executed { get; set; }

        public QueryExecuteContext() {
            Executing = Enumerable.Empty<Action>();
            Executed = Enumerable.Empty<Action>();
        }
    }

    public class EntityController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITokenizer _tokenizer;
        private readonly IRepository<FilterRecord> _filterRepository;
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;
        private readonly IRepository<SortCriterionRecord> _sortCriterioRepository;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public EntityController(
            IContentManager iContentManager,
            ICoeveryServices coeveryServices,
            IProjectionManager projectionManager,
            IContentDefinitionExtension contentDefinitionExtension,
            ITokenizer tokenizer,
            IRepository<FilterRecord> filterRepository,
            IRepository<FilterGroupRecord> filterGroupRepository, 
            IRepository<SortCriterionRecord> sortCriterioRepository) {
            _contentManager = iContentManager;
            Services = coeveryServices;
            _contentDefinitionExtension = contentDefinitionExtension;
            _projectionManager = projectionManager;
            _tokenizer = tokenizer;
            _filterRepository = filterRepository;
            _filterGroupRepository = filterGroupRepository;
            _sortCriterioRepository = sortCriterioRepository;
            Logger = NullLogger.Instance;
        }

        public ICoeveryServices Services { get; private set; }
        public ILogger Logger { get; set; }

        public object Post(string id, ListQueryModel model) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var part = GetProjectionPartRecord(model.ViewId);
            if (part == null) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            id = _contentDefinitionExtension.GetEntityNameFromCollectionName(id);

            var executeContext = new QueryExecuteContext();
            SortQuery(id, part, model, executeContext);
            FilterQuery(id, part, model, executeContext);
            executeContext.Executing.Invoke(action => action(), Logger);

            var totalRecords = _projectionManager.GetCount(part.Record.QueryPartRecord.Id);
            var pageSize = model.Rows;
            var pager = new Pager(Services.WorkContext.CurrentSite, model.Page, pageSize);
            var records = GetLayoutComponents(part, pager.GetStartIndex(), pager.PageSize);

            executeContext.Executed.Invoke(action => action(), Logger);

            var filterDescription = GetFilterDisplayText(id, model);

            var results = new {
                totalRecords = totalRecords,
                rows = records,
                filterDescription = filterDescription
            };
            return results;
        }

        private void SortQuery(string id, ProjectionPart part, ListQueryModel model, QueryExecuteContext context) {

            var existingExecutingAction = context.Executing ?? Enumerable.Empty<Action>();
            var newSortCriteria = CreateSortCriterion(id, model).ToList();
            var exitingSortCriteria = part.Record.QueryPartRecord.SortCriteria;
            Action executingAction = () => newSortCriteria.ForEach(exitingSortCriteria.Add);
            context.Executing = existingExecutingAction.Concat(new[] {executingAction});

            var existingExecutedAction = context.Executed ?? Enumerable.Empty<Action>();
            Action executedAction = () => {
                foreach (var record in newSortCriteria) {
                    exitingSortCriteria.Remove(record);
                    _sortCriterioRepository.Delete(record);
                }
            };
            context.Executing = existingExecutedAction.Concat(new[] {executedAction});
        }

        private IEnumerable<SortCriterionRecord> CreateSortCriterion(string entityName, ListQueryModel model) {

            if (!string.IsNullOrEmpty(model.SortBy)) {
  
                yield return new SortCriterionRecord {
                    Category = entityName.ToPartName() + "ContentFields",
                    Type = entityName.ToPartName() + "." + model.SortBy + ".",
                    State = FormParametersHelper.ToString(new Dictionary<string, string> {
                        {"Sort", (model.Sort == "asc").ToString().ToLower()}
                    }),
                    Position = 0
                };
            }
        }

        private void FilterQuery(string id, ProjectionPart part, ListQueryModel model, QueryExecuteContext context) {
            var existingExecutingAction = context.Executing ?? Enumerable.Empty<Action>();
            model.Filters = model.Filters ?? new FilterData[] {};

            var filterRecords = CreateFilters(id, model);
            var filters = part.Record.QueryPartRecord.FilterGroups.First().Filters;
            Action executingAction = () => filterRecords.ForEach(filters.Add);
            context.Executing = existingExecutingAction.Concat(new[] {executingAction});

            var existingExecutedAction = context.Executed ?? Enumerable.Empty<Action>();
            Action executedAction = () => {
                foreach (var record in filterRecords) {
                    filters.Remove(record);
                    _filterRepository.Delete(record);
                }
            };
            context.Executing = existingExecutedAction.Concat(new[] {executedAction});
        }

        public void Delete(string contentId) {
            var idList = contentId.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var idItem in idList) {
                var contentItem = _contentManager.Get(int.Parse(idItem), VersionOptions.Latest);
                _contentManager.Remove(contentItem);
            }
        }

        private IEnumerable<FilterRecord> CreateFilters(string entityName, ListQueryModel model) {
            IList<FilterRecord> filterRecords = new List<FilterRecord>();
            if (model.IsRelationList) {
                if (model.RelationType == "OneToMany") {
                    var settings = new Dictionary<string, string> {
                        {"Operator", "MatchesAny"},
                        {"Value", model.CurrentItem.ToString("D")}
                    };
                    var relationFilter = new FilterRecord {
                        Category = entityName.ToPartName() + "ContentFields",
                        Type = entityName.ToPartName() + "." + model.RelationId + ".",
                        State = FormParametersHelper.ToString(settings),
                        Description = "Only show entries related to current item."
                    };
                    filterRecords.Add(relationFilter);
                }
            }

            foreach (var filter in model.Filters) {
                if (filter.FormData.Length == 0) {
                    continue;
                }
                var record = new FilterRecord {
                    Category = entityName.ToPartName() + "ContentFields",
                    Type = filter.Type,
                };
                var dictionary = new Dictionary<string, string>();
                foreach (var data in filter.FormData) {
                    if (dictionary.ContainsKey(data.Name)) {
                        dictionary[data.Name] += "&" + data.Value;
                    }
                    else {
                        dictionary.Add(data.Name, data.Value);
                    }
                }
                record.State = FormParametersHelper.ToString(dictionary);
                filterRecords.Add(record);
            }
            return filterRecords;
        }

        private string GetFilterDisplayText(string entityName, ListQueryModel model) {
            var displayText = string.Empty;
            if (model.FilterGroupId == 0) {
                var filterDescriptors = _projectionManager.DescribeFilters()
                    .Where(x => x.Category == entityName.ToPartName() + "ContentFields")
                    .SelectMany(x => x.Descriptors).ToList();
                foreach (var filter in model.Filters) {
                    if (filter.FormData.Length == 0) {
                        continue;
                    }
                    var record = new FilterRecord {
                        Category = entityName.ToPartName() + "ContentFields",
                        Type = filter.Type,
                    };
                    var dictionary = new Dictionary<string, string>();
                    foreach (var data in filter.FormData) {
                        if (dictionary.ContainsKey(data.Name)) {
                            dictionary[data.Name] += "&" + data.Value;
                        }
                        else {
                            dictionary.Add(data.Name, data.Value);
                        }
                    }
                    record.State = FormParametersHelper.ToString(dictionary);
                    var descriptor = filterDescriptors.First(x => x.Type == filter.Type);
                    displayText += descriptor.Display(new FilterContext {State = FormParametersHelper.ToDynamic(record.State)}).Text;
                }
            }
            return displayText;
        }

        private ProjectionPart GetProjectionPartRecord(int viewId) {
            if (viewId == -1) {
                return null;
            }
            var projectionContentItem = _contentManager.Get(viewId, VersionOptions.Latest);
            var part = projectionContentItem.As<ProjectionPart>();
            return part;
        }

        private IEnumerable<JObject> GetLayoutComponents(ProjectionPart part, int skipCount, int pageCount) {
            // query
            var query = part.Record.QueryPartRecord;

            // applying layout
            var layout = part.Record.LayoutRecord;
            var tokens = new Dictionary<string, object> {{"Content", part.ContentItem}};
            var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
            var fieldDescriptors = layout.Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new {Descriptor = d, Property = p}).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            fieldDescriptors = fieldDescriptors.Where(c => c != null).ToList();
            var tokenizedDescriptors = fieldDescriptors.Select(fd => new {fd.Descriptor, fd.Property, State = FormParametersHelper.ToDynamic(_tokenizer.Replace(fd.Property.State, tokens))}).ToList();

            // execute the query
            var contentItems = _projectionManager.GetContentItems(query.Id, skipCount, pageCount).ToList();

            // sanity check so that content items with ProjectionPart can't be added here, or it will result in an infinite loop
            contentItems = contentItems.Where(x => !x.Has<ProjectionPart>()).ToList();

            var layoutComponents = contentItems.Select(
                contentItem => {
                    var result = new JObject();
                    result["ContentId"] = contentItem.Id;
                    tokenizedDescriptors.ForEach(
                        d => {
                            var fieldContext = new PropertyContext {
                                State = d.State,
                                Tokens = tokens
                            };
                            var shape = d.Descriptor.Property(fieldContext, contentItem);
                            string text = (shape == null) ? string.Empty : shape.ToString();
                            var filedName = d.Property.GetFiledName();
                            result[filedName] = text;
                        });
                    return result;
                }).ToList();
            return layoutComponents;
        }
    }
}