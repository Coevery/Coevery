using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Core.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Forms.Services;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System.Linq;
using Orchard.Tokens;

namespace Coevery.Core.Controllers {
    public class CommonController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITokenizer _tokenizer;
        private readonly IGridService _gridService;
        private readonly IRepository<FilterRecord> _filterRepository;
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;

        public CommonController(
            IContentManager iContentManager,
            IOrchardServices orchardServices,
            IProjectionManager projectionManager,
            ITokenizer tokenizer,
            IGridService gridService,
            IRepository<FilterRecord> filterRepository,
            IRepository<FilterGroupRecord> filterGroupRepository) {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionManager = projectionManager;
            _tokenizer = tokenizer;
            _gridService = gridService;
            _filterRepository = filterRepository;
            _filterGroupRepository = filterGroupRepository;
        }

        public IOrchardServices Services { get; private set; }

        public object Post(string id, ListQueryModel model) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);

            var part = GetProjectionPartRecord(model.ViewId);
            var totalNumber = 0;
            IEnumerable<JObject> entityRecords = null;
            if (part != null) {
                if (model.Filters == null) {
                    model.Filters = new FilterData[] { };
                }
                
                var filterRecords = CreateFilters(id, model);
                var filters = part.Record.QueryPartRecord.FilterGroups.First().Filters;
                filterRecords.ForEach(filters.Add);

                totalNumber = _projectionManager.GetCount(part.Record.QueryPartRecord.Id);
                //var skipCount = model.rows*(model.page - 1);
                //var pageCount = totalNumber <= model.rows*model.page ? totalNumber - model.rows*(model.page - 1) : model.rows;
                entityRecords = GetLayoutComponents(part, 0, 0);

                foreach (var record in filterRecords) {
                    filters.Remove(record);
                    if (model.FilterGroupId == 0) {
                        _filterRepository.Delete(record);
                    }
                }
            }

            if (entityRecords == null || !entityRecords.Any()) {
                return new {
                    total = 0,
                    page = 0,
                    records = 0,
                    rows = string.Empty
                };
            }
            var postsortPage = _gridService.GetSortedRows(model.Sidx, model.Sord, entityRecords);
            //var json = JsonConvert.SerializeObject(returnResult);
            //var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};
            
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalNumber / model.Rows)),
                page = model.Page,
                records = entityRecords.Count(),
                rows = _gridService.GetPagedRows(model.Page, model.Rows, postsortPage)
            };
        }

        public void Delete(string contentId) {
            var idList = contentId.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries);
            foreach (var idItem in idList) {
                var contentItem = _contentManager.Get(int.Parse(idItem), VersionOptions.Latest);
                _contentManager.Remove(contentItem);
            }
        }

        private IList<FilterRecord> CreateFilters(string entityName, ListQueryModel model) {
            IList<FilterRecord> filterRecords;
            if (model.FilterGroupId == 0) {
                filterRecords = new List<FilterRecord>();
                foreach (var filter in model.Filters) {
                    if (filter.FormData.Length == 0) {
                        continue;
                    }
                    var record = new FilterRecord {
                        Category = entityName + "ContentFields",
                        Type = filter.Type,
                    };
                    var dictionary = filter.FormData.ToDictionary(x => x.Name, x => x.Value);
                    record.State = FormParametersHelper.ToString(dictionary);
                    filterRecords.Add(record);
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
                            var text = shape == null ? string.Empty : shape.ToString();
                            var filedName = d.Property.GetFiledName();
                            result[filedName] = text;
                        });
                    return result;
                }).ToList();
            return layoutComponents;
        }
    }
}