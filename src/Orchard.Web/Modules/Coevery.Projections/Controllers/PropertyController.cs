using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (id <= 0)
                return null;

            var listViewPart = _contentManager.Get<ListViewPart>(id, VersionOptions.Latest);
            var projectionPart = listViewPart.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;
            if (queryPartRecord.Layouts.Count == 0)
                return null;
            string category = listViewPart.ItemContentType + "ContentFields";
            var allFields = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors).Where(c => c.Category == category);
            var fieldDescriptors = queryPartRecord.Layouts[0].Properties.OrderBy(p => p.Position).Select(p => allFields.Select(d => new {Descriptor = d, Property = p}).FirstOrDefault(x => x.Descriptor.Type == p.Type)).ToList();
            return fieldDescriptors.Select(x => new {FieldName = x.Descriptor.Type, DisplayName = x.Descriptor.Name.Text});
        }
    }
}