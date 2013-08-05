using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Core.Services;
using Coevery.FormDesigner.Models;
using Orchard.ContentManagement.MetaData;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.FormDesigner.Controllers {
    public class LayoutController : ApiController {
        private IContentDefinitionManager _contentDefinitionManager;
        private readonly ITemplateViewService _templateViewService;
        private readonly string _alwaysInLayoutKey = "CoeveryTextFieldSettings.AlwaysInLayout";
        public LayoutController(IContentDefinitionManager contentDefinitionManager, 
            ITemplateViewService templateViewService) {
            _contentDefinitionManager = contentDefinitionManager;
            _templateViewService = templateViewService;
        }

        // GET api/leads/lead/5
        public virtual object Get(string id) {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            if (contentTypeDefinition == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;

            var layoutInfo = new {
                id,
                layout
            };
            return layoutInfo;
        }

        // POST api/metadata/field
        public virtual HttpResponseMessage Post(string id , ICollection<Section> data) {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
            if (contentTypeDefinition == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var layout = GetLayout(contentTypeDefinition,data);
            if (string.IsNullOrEmpty(layout)) return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            if (contentTypeDefinition.Settings.ContainsKey("Layout"))
            {
                contentTypeDefinition.Settings["Layout"] = layout;
            }
            else
            {
                contentTypeDefinition.Settings.Add("Layout", layout);
            }
            _contentDefinitionManager.StoreTypeDefinition(contentTypeDefinition);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private string GetLayout(ContentTypeDefinition contentTypeDefinition, ICollection<Section> data)
        {
            //check field valid
            if(contentTypeDefinition.Parts.Any()) {
                var part = contentTypeDefinition.Parts.First();
                var fields = part.PartDefinition.Fields;
                var columns = data.SelectMany(c => c.Rows).SelectMany(c => c.Columns);
                if(fields.Any(f => f.Settings.ContainsKey(_alwaysInLayoutKey) 
                    && bool.Parse(f.Settings[_alwaysInLayoutKey]) 
                    && !columns.Select(c => c.Field.FieldName).Contains(f.Name))) {
                    return string.Empty;
                }
                var validColumns = columns.Where(c => fields.Select(d=>d.Name).Contains(c.Field.FieldName)).ToList();
                validColumns.ForEach(c=>c.Field.IsValid = true);
            }
            

            ViewDataDictionary viewData = new ViewDataDictionary();
            viewData.Add("Layout",data);
            string layout = _templateViewService
                .RenderView("Coevery.FormDesigner", "FormTemplate", "FormDesignerLayout", viewData);
            return layout;
        }
    }
}