using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Utility.Extensions;
using Coevery.Core;

namespace Coevery.Projections.Controllers {
    public class ViewModelController : ApiController {
        private readonly IProjectionService _projectionService;
        private readonly IContentManager _contentManager;
         public ViewModelController(IProjectionService projectionService, 
             IContentManager contentManager) {
             _projectionService = projectionService;
             _contentManager = contentManager;
         }

        public IEnumerable<JObject> Get(int id)
        {
            var properties = GetProperties(id);
            List<JObject> re = new List<JObject>();
            properties.ToList().Select(c => c.GetFiledName()).ToList().ForEach(c =>
            {
                 JObject reObJ = new JObject();
                 reObJ["FieldName"] = c;
                 re.Add(reObJ);
             });
            return re;
        }

        private IEnumerable<PropertyRecord> GetProperties(int projectionId)
        {
            IList<PropertyRecord> properties = new List<PropertyRecord>();
            if (projectionId == -1)
                return properties;

            var projectionItem = _contentManager.Get(projectionId, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0)
                return properties;
            properties = queryPartRecord.Layouts[0].Properties;
            return properties;
        }
    }
}