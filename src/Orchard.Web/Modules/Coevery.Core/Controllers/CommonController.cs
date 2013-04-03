using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Forms.Services;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Tokens;
using Orchard.WebApi.Common;
using System.Linq.Expressions;
using System.Linq;

namespace Coevery.Core.Controllers
{
    public class CommonController :ApiController
    {
        private IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITokenizer _tokenizer;

        public CommonController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IProjectionManager projectionManager, 
            ITokenizer tokenizer)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionManager = projectionManager;
            _tokenizer = tokenizer;
        }

        public IOrchardServices Services { get; private set; }

        // GET api/leads/lead
        public IEnumerable<object> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            
            //List<object> re = new List<object>();
           
            //string moduleName = id;
            //var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            //var contentType = pluralService.Singularize(id);
            //var contentItems = _contentManager.Query(contentType).List();
            //foreach (var contentItem in contentItems)
            //{
            //    var contentPart = contentItem.Parts.FirstOrDefault(t => t.PartDefinition.Name == contentType);
            //    if (contentPart != null)
            //    {
                   
            //    }
            //}
            var re = this.GetLayoutComponents();
            return re;
        }

        private IEnumerable<JObject> GetLayoutComponents()
        {
            var contentItem1 = _contentManager.Get(40, VersionOptions.Published);
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