using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Relationship.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Projections.Models;
using Orchard.Localization;
using Orchard.Projections.Services;

namespace Coevery.Relationship.Settings
{
    public class ReferenceFieldEditorEvents : FieldEditorEvents
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentManager _contentManager;
        private readonly IRelationshipService _relationshipService;
        public Localizer T { get; set; }
        public ReferenceFieldEditorEvents(IContentDefinitionService contentDefinitionService, 
            IContentManager contentManager,
            IRelationshipService relationshipService) {
            _contentDefinitionService = contentDefinitionService;
            _contentManager = contentManager;
            _relationshipService = relationshipService;
            T = NullLocalizer.Instance;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "ReferenceField" ||
                definition.FieldDefinition.Name == "ReferenceFieldCreate")
            {
                var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                var model = definition.Settings.GetModel<ReferenceFieldSettings>();
                model.ContentTypeList = metadataTypes.Select(item => new SelectListItem
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
                UpdateSettings(model, builder, "ReferenceFieldSettings");
                builder.WithSetting("ReferenceFieldSettings.DisplayAsLink", model.DisplayAsLink.ToString(CultureInfo.InvariantCulture));
            }
            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel)
        {
            if (builder.FieldType != "ReferenceField") {
                yield break;
            }
            var model = new ReferenceFieldSettings();
            if (updateModel.TryUpdateModel(model, "ReferenceFieldSettings", null, null)) {
                model.QueryId = CreateQuery(model.ContentTypeName.ToString(CultureInfo.InvariantCulture));

                if (model.RelationshipId <= 0) {
                    model.RelationshipId = _relationshipService.CreateOneToManyRelationship(builder.Name, model.RelationshipName, model.ContentTypeName, partName);
                }
                
                if (model.QueryId <= 0 || model.RelationshipId <= 0) {
                    updateModel.AddModelError("QueryOrRelation",T("Invalid Query or Relationship Id"));
                    yield break;
                }

                UpdateSettings(model, builder, "ReferenceFieldSettings");
                builder.WithSetting("ReferenceFieldSettings.DisplayAsLink", model.DisplayAsLink.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.ContentTypeName", model.ContentTypeName.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.RelationshipName", model.RelationshipName.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.RelationshipId", model.RelationshipId.ToString("D"));
                builder.WithSetting("ReferenceFieldSettings.QueryId", model.QueryId.ToString("D"));
            }
            yield return DefinitionTemplate(model);
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
