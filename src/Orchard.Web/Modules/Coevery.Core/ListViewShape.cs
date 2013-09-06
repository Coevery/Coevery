using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using Coevery.Core.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using System.IO;
using System.Web.Mvc;
using Orchard.Projections.Models;

namespace Coevery.Core
{
    public class ListViewShape : IShapeTableProvider
    {
        private IContentDefinitionManager _contentDefinitionManager;
        private IContentManager _contentManager;
        private readonly IViewPartService _projectionService;

        public ListViewShape(IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IViewPartService projectionService)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _projectionService = projectionService;
        }
        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("Content_Edit")
                .OnDisplaying(displaying =>
                {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content_Edit__Default");
                });
        }

        [Shape]
        public void ngGrid(dynamic Display, TextWriter Output, HtmlHelper Html, IEnumerable<dynamic> Items, String ContentType)
        {
            Output.WriteLine("<section class=\"gridStyle\" ng-grid=\"gridOptions\"></section>");

            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(ContentType);
            int viewId = _projectionService.GetProjectionId(ContentType);
            var columns = this.GetViewColumns(viewId);

            string moduleName = ContentType;
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsSingular(moduleName))
            {
                moduleName = pluralService.Pluralize(moduleName);
            }
            this.OutPutListViewColumns(Output,columns,moduleName);
        }

        private void OutPutListViewColumns(TextWriter Output, IEnumerable<PropertyRecord> columns, string moduleName)
        {

            Output.Write("<script> function getColumnDefs(localize){");
            string cellVarTemp = "var cellTemplate{0} = '<div><a href =\"Coevery#/{1}/{{{{row.entity.ContentId}}}}\" class=\"ngCellText\">{{{{row.entity.{0}}}}}</a></div>';";
            foreach (var col in columns)
            {
                string cellTemplae = string.Empty;
                if (col.LinkToContent)
                {
                    string cellVar = string.Format(cellVarTemp, col.Description, moduleName);
                    Output.Write(cellVar);
                }
            }

            Output.Write("var columnDefs = [");

            int index = 0;
            Output.Write("{ field: 'ContentId', displayName: 'Actions',width: 150,cellTemplate: '<div style=\"margin-top: 5px;margin-left: 5px;\"><span class=\"span2\"><a ng-click=\"edit(row.getProperty(col.field))\">Eidt</a></span><span class=\"span2\"></span><span class=\"span2\"><a ng-click=\"delete(row.getProperty(col.field))\">Remove</a></span></div>' },");
            Output.Write("{ field: 'ContentId', displayName: localize.getLocalizedString('Id') },");
            foreach (var col in columns)
            {
                var colTemp = "{{ field: '{0}', displayName: localize.getLocalizedString('{0}') {1} }},";
                if (index == columns.Count() - 1)
                {
                    colTemp = "{{ field: '{0}', displayName: localize.getLocalizedString('{0}') {1} }}";
                }
                string cellTemplae = string.Empty;
                if (col.LinkToContent)
                {
                    cellTemplae = ",cellTemplate: cellTemplate" + col.Description;
                }
                string text = string.Format(colTemp, col.Description, cellTemplae);
                Output.Write(text);
                index++;

            }
            Output.Write(@"]; return columnDefs;}");
            Output.Write("</script> ");
        }

        private IEnumerable<PropertyRecord> GetViewColumns(int viewId)
        {
            List<PropertyRecord> re = new List<PropertyRecord>();
            if (viewId == -1) return re;
            var projectionItem = _contentManager.Get(viewId, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0) return re;
            var properties = queryPartRecord.Layouts[0].Properties;
            re.AddRange(properties);
            return re;
        }

    }
}
