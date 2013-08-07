using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Projections.Models;
using Orchard.Projections.Services;

namespace Coevery.Fields.Settings
{
    public class ReferenceFieldEditorEvents : FieldEditorEvents
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        public ReferenceFieldEditorEvents(IContentDefinitionService contentDefinitionService, 
            IContentManager contentManager,
            IProjectionManager projectionManager) {
            _contentDefinitionService = contentDefinitionService;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "ReferenceField" ||
                definition.FieldDefinition.Name == "ReferenceFieldCreate")
            {
                var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                var model = definition.Settings.GetModel<ReferenceFieldSettings>();
                model.ContentTypeList = metadataTypes.Where(d=>d.Name != definition.FieldDefinition.ContentType).Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name,
                    Selected = item.Name == model.ContentTypeName
                }).ToList();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "ReferenceField") {
                yield break;
            }
            var model = new ReferenceFieldSettings();
            
            if (updateModel.TryUpdateModel(model, "ReferenceFieldSettings", null, null)) {
                SettingsDictionary setting = null;
                var field = builder.GetType().GetField("_settings",BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                if (field != null)
                {
                    setting = (SettingsDictionary)field.GetValue(builder);
                }
                int queryId = setting != null && setting.ContainsKey("ReferenceFieldSettings.QueryId")?
                    int.Parse(setting["ReferenceFieldSettings.QueryId"]) : 0;
                if (queryId <= 0)
                {
                    queryId = CreateQuery(model.ContentTypeName.ToString(CultureInfo.InvariantCulture));
                }
                UpdateSettings(model, builder, "ReferenceFieldSettings");
                builder.WithSetting("ReferenceFieldSettings.ContentTypeName", model.ContentTypeName.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.QueryId", queryId.ToString());
                builder.WithSetting("ReferenceFieldSettings.DisplayAsLink", model.DisplayAsLink.ToString(CultureInfo.InvariantCulture));
            }
            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel)
        {
            return PartFieldEditorUpdate(builder, updateModel);
        }


        private string GetContentTypeFilterState(string entityType)
        {
            string format = @"<Form>
                  <Description></Description>
                  <ContentTypes>{0}</ContentTypes>
                  <__RequestVerificationToken>POESz5zBfaUfKi7nV-DN7HBjHfMa6SDP08I_cFQu5y6_iV_PXniWPAJQOFVXsajUk2hk_QMrKZ8fLDCxATbMmuJuNUK_rhBRq2DIld2IJ0E-yGca8Jw8Ma_dWrri63fgR5hmVq1rfuOGFtEM1YJaZUSlgOHVe7RH1GKag_vA2nQ1</__RequestVerificationToken>
                </Form>";
            return string.Format(format, entityType);
        }

        private int CreateQuery(string entityType) 
        {
            var queryItem = _contentManager.New("Query");
            var queryPart = queryItem.As<QueryPart>();
            _contentManager.Create(queryItem, VersionOptions.Draft);
            var filterGroup = new FilterGroupRecord();
            queryPart.Record.FilterGroups.Clear();
            queryPart.Record.FilterGroups.Add(filterGroup);
            var filterRecord = new FilterRecord
            {
                Category = "Content",
                Type = "ContentTypes",
                Position = filterGroup.Filters.Count,
                State = GetContentTypeFilterState(entityType)
            };
            filterGroup.Filters.Add(filterRecord);
            _contentManager.Publish(queryItem);
            return queryItem.Id;
        }
    }
}
