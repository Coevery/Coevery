using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Coevery.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Forms.Services;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System.Linq;
using Orchard.Tokens;

namespace Coevery.Core.Controllers
{
    public class CommonController :ApiController
    {
        private IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITokenizer _tokenizer;
        private readonly IViewPartService _projectionService;

        public CommonController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IProjectionManager projectionManager, 
            ITokenizer tokenizer,
            IViewPartService projectionService)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionManager = projectionManager;
            _tokenizer = tokenizer;
            _projectionService = projectionService;
        }

        public IOrchardServices Services { get; private set; }

        // GET api/leads/lead
        public HttpResponseMessage Get(string id, int pageSize,int page)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);
            var part = GetQueryPartRecord(id);
            IEnumerable<JObject> entityRecords = new List<JObject>();
            int totalNumber = 0;
            if (part != null) {
                totalNumber = _projectionManager.GetCount(part.Record.QueryPartRecord.Id);
                int skipCount = pageSize * (page - 1);
                int pageCount = totalNumber <= pageSize * page ? totalNumber - pageSize * (page - 1) : pageSize;
                entityRecords = this.GetLayoutComponents(part, skipCount, pageCount);
            }
            var returnResult = new { TotalNumber = totalNumber, EntityRecords = entityRecords };
            var json = JsonConvert.SerializeObject(returnResult);

            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }

        public void Delete(string id) {
            string[] idList = id.Split(new char[] { ',' });
            foreach (var idItem in idList) {
                var contentItem = _contentManager.Get(int.Parse(idItem), VersionOptions.Latest);
                _contentManager.Remove(contentItem);
            } 
        }

        private ProjectionPart GetQueryPartRecord(string entityType)
        {
            int viewId = _projectionService.GetProjectionId(entityType);
            if (viewId == -1) return null;

            var projectionContentItem = _contentManager.Get(viewId, VersionOptions.Latest);
            var part = projectionContentItem.As<ProjectionPart>();
            return part;
        }

        private IEnumerable<JObject> GetLayoutComponents(ProjectionPart part, int skipCount, int pageCount) 
        {
            // query
            var query = part.Record.QueryPartRecord;

            // applying layout
            var layout = part.Record.LayoutRecord;
            var tokens = new Dictionary<string, object> { { "Content", part.ContentItem } };
            var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
            var fieldDescriptors = layout.Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new { Descriptor = d, Property = p }).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            fieldDescriptors = fieldDescriptors.Where(c => c != null).ToList();
            var tokenizedDescriptors = fieldDescriptors.Select(fd => new { fd.Descriptor, fd.Property, State = FormParametersHelper.ToDynamic(_tokenizer.Replace(fd.Property.State, tokens)) }).ToList();

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