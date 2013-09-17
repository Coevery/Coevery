using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;
using Coevery.Core;

namespace Coevery.Projections.Controllers {
    public class PropertyController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public PropertyController(IContentManager contentManager,
            IProjectionManager projectionManager) {
            _contentManager = contentManager;
            _projectionManager = projectionManager;
        }

        public IEnumerable<object> Get(int id) {
            var properties = GetProperties(id);
            return properties.Select(c => new {FieldName = c.Type});
        }

        private IEnumerable<PropertyRecord> GetProperties(int projectionId) {
            IList<PropertyRecord> properties = new List<PropertyRecord>();
            if (projectionId <= 0)
                return properties;

            var listViewPart = _contentManager.Get<ListViewPart>(projectionId, VersionOptions.Latest);
            var projectionPart = listViewPart.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;
            if (queryPartRecord.Layouts.Count == 0)
                return properties;
            string category = listViewPart.ItemContentType + "ContentFields";
            var allFields = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors).Where(c => c.Category == category);
            properties = queryPartRecord.Layouts[0].Properties.Where(c => allFields.Select(d => d.Type).Contains(c.Type)).ToList();
            return properties;
        }
    }
}