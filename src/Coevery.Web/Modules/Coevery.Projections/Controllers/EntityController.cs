using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using Coevery.Common.Extensions;
using Coevery.Common.Services;
using Coevery.Common.ViewModels;
using Coevery.ContentManagement.FieldStorage;
using Coevery.Projections.Services;
using Newtonsoft.Json;
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
    public class EntityController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITokenizer _tokenizer;
        private readonly IGridService _gridService;
        private readonly IRepository<FilterRecord> _filterRepository;
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public EntityController(
            IContentManager iContentManager,
            ICoeveryServices coeveryServices,
            IProjectionManager projectionManager,
            IContentDefinitionExtension contentDefinitionExtension,
            ITokenizer tokenizer,
            IGridService gridService,
            IRepository<FilterRecord> filterRepository,
            IRepository<FilterGroupRecord> filterGroupRepository) {
            _contentManager = iContentManager;
            Services = coeveryServices;
            _contentDefinitionExtension = contentDefinitionExtension;
            _projectionManager = projectionManager;
            _tokenizer = tokenizer;
            _gridService = gridService;
            _filterRepository = filterRepository;
            _filterGroupRepository = filterGroupRepository;
        }

        public ICoeveryServices Services { get; private set; }

        public object Post(string id, ListQueryModel model) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var part = GetProjectionPartRecord(model.ViewId);
            if (part == null) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            id = _contentDefinitionExtension.GetEntityNameFromCollectionName(id);
            string filterDescription = null;
            
            return GetFilteredRecords(id, part, out filterDescription, model, p => {
                _gridService.GenerateSortCriteria(id, model.Sidx, model.Sord, p.Record.QueryPartRecord.Id);
                var totalRecords = _projectionManager.GetCount(p.Record.QueryPartRecord.Id);
                var pageSize = model.Rows;
                var totalPages = (int) Math.Ceiling((float) totalRecords/(float) pageSize);
                var pager = new Pager(Services.WorkContext.CurrentSite, model.Page, pageSize);
                var records = GetLayoutComponents(p, pager.GetStartIndex(), pager.PageSize);

                return new {
                    totalPages = totalPages,
                    page = model.Page,
                    totalRecords = totalRecords,
                    rows = records,
                    filterDescription = filterDescription
                };
            });
        }

        private object GetFilteredRecords(string id, ProjectionPart part, out string filterDescription, ListQueryModel model, Func<ProjectionPart, object> query) {
            model.Filters = model.Filters ?? new FilterData[] {};

            var filterRecords = CreateFilters(id, model, out filterDescription);
            var filters = part.Record.QueryPartRecord.FilterGroups.First().Filters;
            filterRecords.ForEach(filters.Add);
            try {
                return query(part);
            }

            finally {
                foreach (var record in filterRecords) {
                    filters.Remove(record);
                    if (model.FilterGroupId == 0) {
                        _filterRepository.Delete(record);
                    }
                }
            }
        }

        public void Delete(string contentId) {
            var idList = contentId.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var idItem in idList) {
                var contentItem = _contentManager.Get(int.Parse(idItem), VersionOptions.Latest);
                _contentManager.Remove(contentItem);
            }
        }

        private IList<FilterRecord> CreateFilters(string entityName, ListQueryModel model, out string filterDescription) {
            IList<FilterRecord> filterRecords;
            filterDescription = string.Empty;
            if (model.FilterGroupId == 0) {
                var allDescriptors = _projectionManager.DescribeFilters()
                    .SelectMany(x => x.Descriptors).ToList();
                filterRecords = new List<FilterRecord>();
                if (model.IsRelationList) {
                    if (model.RelationType == "OneToMany") {
                        var settings = new Dictionary<string, string> {
                            {"Operator","MatchesAny"},
                            {"Value",model.CurrentItem.ToString("D")}
                        };
                        string category = entityName.ToPartName() + "ContentFields";
                        var relationFilter = new FilterRecord {
                            Category = category,
                            Type = entityName.ToPartName() + "." + model.RelationId + ".",
                            State = FormParametersHelper.ToString(settings),
                            Description = "Only show entries related to current item."
                        };
                        filterRecords.Add(relationFilter);
                        var descriptor = allDescriptors.First(x => x.Category == category && x.Type == relationFilter.Type);
                        filterDescription += descriptor.Display(new FilterContext { State = FormParametersHelper.ToDynamic(relationFilter.State) }).Text;
                    }
                }

                foreach (var filter in model.Filters) {
                    if (filter.FormData.Length == 0) {
                        continue;
                    }
                    var record = new FilterRecord {
                        Category = filter.Category,
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
                    var descriptor = allDescriptors.First(x => x.Category == filter.Category && x.Type == filter.Type);
                    filterDescription += descriptor.Display(new FilterContext {State = FormParametersHelper.ToDynamic(record.State)}).Text;
                }
            }
            else {
                filterRecords = _filterGroupRepository.Get(model.FilterGroupId).Filters;
            }
            return filterRecords;
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
                            var fieldName = d.Property.GetFieldName();
                            result[fieldName] = text;
                        });
                    if (layout.Category == "Grids" && layout.Type == "Tree") {
                        var parentFieldName = FormParametersHelper.FromString(layout.State)["ParentField"].GetFieldName();
                        var parentValue = GetParentId(contentItem, parentFieldName);
                        result["parent"] = parentValue;
                        result["expanded"] = true;
                    }
                    return result;
                }).ToList();
            return layout.Category == "Grids" && layout.Type == "Tree" ? layoutComponents.Select(record => {
                record["level"] = GetLevel(layoutComponents, record["parent"].Value<int?>());
                record["isLeaf"] = IsLeaf(layoutComponents, record["ContentId"].Value<int>());
                return record;
            })
            : layoutComponents;
        }

        private static bool IsLeaf(IEnumerable<JObject> contentItems, int currentId) {
            return !contentItems.Any(record => {
                var currentParent = record["parent"].Value<int?>();
                return currentParent.HasValue && currentParent.Value == currentId;
            });
        }

        private static int GetLevel(IEnumerable<JObject> contentItems, int? parentId) {
            var currentId = parentId;
            var level = 0;
            while (currentId.HasValue) {
                var parentItem = contentItems.FirstOrDefault(item => item["ContentId"].Value<int>() == currentId.Value);
                if (parentItem == null) {
                    throw new JsonReaderException();
                }
                currentId = parentItem["parent"].Value<int?>();
                level++;
            }
            return level;
        }

        private static int? GetParentId(ContentItem contentItem, string parentFieldName) {
            var entityPart = contentItem.Parts
                            .FirstOrDefault(p => p.PartDefinition.Name == contentItem.ContentType.ToPartName());
            if (entityPart == null) {
                throw new InstanceNotFoundException("Entity part not found!");
            }
            var parentField = entityPart.Fields.FirstOrDefault(f => f.Name == parentFieldName);
            if (parentField == null) {
                throw new InstanceNotFoundException("Parent field not found!");
            }
            return parentField.Storage.Get<int?>();
        }
    }
}