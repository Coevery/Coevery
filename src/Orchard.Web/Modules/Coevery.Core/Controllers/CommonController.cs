using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
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
        public IEnumerable<object> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);
            var re = this.GetLayoutComponents(id);
            return re;
        }

        public void Delete(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            _contentManager.Remove(contentItem);
        }

        private IEnumerable<JObject> GetLayoutComponents(string entityType)
        {
            int viewId = _projectionService.GetProjectionId(entityType);
            if(viewId == -1)return  new LinkedList<JObject>();

            var contentItem1 = _contentManager.Get(viewId, VersionOptions.Latest);
            var part = contentItem1.As<ProjectionPart>();
            var query = part.Record.QueryPartRecord;
            // applying layout
            var layout = part.Record.LayoutRecord;
            var tokens = new Dictionary<string, object> { { "Content", part.ContentItem } };
            var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
            var fieldDescriptors = layout.Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new { Descriptor = d, Property = p }).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            var tokenizedDescriptors = fieldDescriptors.Select(fd => new { fd.Descriptor, fd.Property, State = FormParametersHelper.ToDynamic(_tokenizer.Replace(fd.Property.State, tokens)) }).ToList();

            // execute the query
            var contentItems = _projectionManager.GetContentItems(query.Id, 0, 20).ToList();

            // sanity check so that content items with ProjectionPart can't be added here, or it will result in an infinite loop
            contentItems = contentItems.Where(x => !x.Has<ProjectionPart>()).ToList();

            var layoutComponents = contentItems.Select(
                contentItem =>
                {

                    var contentItemMetadata = Services.ContentManager.GetItemMetadata(contentItem);
                    var result = new JObject();
                    result["ContentId"] = contentItem.Id;
                     tokenizedDescriptors.ForEach(
                        d =>
                        {
                            var fieldContext = new PropertyContext
                            {
                                State = d.State,
                                Tokens = tokens
                            };
                            var val = d.Descriptor.Property(fieldContext, contentItem);
                            var text = val==null? string.Empty : val.ToString();
                            result[d.Property.Description] = text;
                        });
                    return result;


                }).ToList();
            return layoutComponents;
        }
    
    }
}