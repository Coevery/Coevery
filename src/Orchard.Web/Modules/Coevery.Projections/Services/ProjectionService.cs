using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Coevery.Entities.Services;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Title.Models;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Projections.Descriptors.Property;
using Orchard.UI.Notify;

namespace Coevery.Projections.Services {
    public class ProjectionService : IProjectionService {
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IFieldToPropertyStateProvider> _fieldToPropertyStateProviders;
        private readonly IFormManager _formManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ProjectionService(
            IOrchardServices services,
            IProjectionManager projectionManager,
            IContentManager contentManager,
            IFormManager formManager,
            IEnumerable<IFieldToPropertyStateProvider> fieldToPropertyStateProviders,
            IContentDefinitionManager contentDefinitionManager) {
            _projectionManager = projectionManager;
            _contentManager = contentManager;
            _formManager = formManager;
            _fieldToPropertyStateProviders = fieldToPropertyStateProviders;
            Services = services;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public IEnumerable<PropertyDescriptor> GetFieldDescriptors(string entityType) {
            var category = entityType + "ContentFields";
            var fieldDescriptors = _projectionManager.DescribeProperties().Where(x => x.Category == category).SelectMany(x => x.Descriptors).ToList();
            return fieldDescriptors;
        }

        public ProjectionEditViewModel GetProjectionViewModel(int id) {
            var viewModel = new ProjectionEditViewModel();
            //Get Projection&QueryPart
            var projectionItem = _contentManager.Get(id, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryId = projectionPart.Record.QueryPartRecord.Id;
            var queryPart = _contentManager.Get<QueryPart>(queryId, VersionOptions.Latest);

            var listViewPart = projectionItem.As<ListViewPart>();
            viewModel.Id = id;
            viewModel.ItemContentType = listViewPart.ItemContentType;
            viewModel.DisplayName = listViewPart.As<TitlePart>().Title;
            viewModel.VisableTo = listViewPart.VisableTo;
            viewModel.PageRowCount = projectionPart.Record.ItemsPerPage;
            viewModel.IsDefault = listViewPart.IsDefault;

            //Get AllFields
            viewModel.Fields = GetFieldDescriptors(listViewPart.ItemContentType);

            var sortCriterion = queryPart.SortCriteria.FirstOrDefault();
            if (sortCriterion != null) {
                var state = FormParametersHelper.ToDynamic(sortCriterion.State);
                viewModel.SortedBy = state.Type;
                var ascending = (bool) state.Sort;
                viewModel.SortMode = ascending ? "Asc" : "Desc";
            }

            return viewModel;
        }

        public string UpdateViewOnEntityAltering(string entityName) {
            var entityType = _contentDefinitionManager.GetTypeDefinition(entityName);
            var listViewParts = _contentManager.Query<ListViewPart, ListViewPartRecord>()
                .Where(record => record.ItemContentType == entityName).List();
            if (entityType == null || listViewParts == null || !listViewParts.Any()) {
                return "Invalid entity name!";
            }
            var category = entityName + "ContentFields";
            const string settingName = "CoeveryTextFieldSettings.IsDispalyField";
            foreach (var view in listViewParts) {
                var projection = view.As<ProjectionPart>().Record;
                var layout = projection.LayoutRecord;
                var pickedFileds = (from field in layout.Properties
                                    select field.Type).ToArray();
                layout.Properties.Clear();
                UpdateLayoutProperties(entityName,ref layout,category,settingName,pickedFileds);
                layout.State = GetLayoutState(projection.QueryPartRecord.Id, layout.Properties.Count, layout.Description);
            }
            return null;
        }

        public int EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFileds) {
            ListViewPart listViewPart;
            ProjectionPart projectionPart;
            QueryPart queryPart;
            if (id == 0) {
                listViewPart = _contentManager.New<ListViewPart>("ListViewPage");
                listViewPart.ItemContentType = viewModel.ItemContentType;
                queryPart = _contentManager.New<QueryPart>("Query");

                var layout = new LayoutRecord {
                    Category = "Html",
                    Type = "ngGrid",
                    Description = "DefaultLayoutFor" + queryPart.Name,
                    Display = 1
                };

                queryPart.Layouts.Add(layout);
                projectionPart = listViewPart.As<ProjectionPart>();
                projectionPart.Record.LayoutRecord = layout;
                projectionPart.Record.QueryPartRecord = queryPart.Record;

                var filterGroup = new FilterGroupRecord();
                queryPart.Record.FilterGroups.Add(filterGroup);
                var filterRecord = new FilterRecord {
                    Category = "Content",
                    Type = "ContentTypes",
                    Position = filterGroup.Filters.Count,
                    State = GetContentTypeFilterState(viewModel.ItemContentType)
                };
                filterGroup.Filters.Add(filterRecord);

                _contentManager.Create(queryPart.ContentItem);
                _contentManager.Create(projectionPart.ContentItem);
            }
            else {
                listViewPart = _contentManager.Get<ListViewPart>(id, VersionOptions.Latest);
                projectionPart = listViewPart.As<ProjectionPart>();
                var queryId = projectionPart.Record.QueryPartRecord.Id;
                queryPart = _contentManager.Get<QueryPart>(queryId, VersionOptions.Latest);
            }

            if (pickedFileds == null) {
                pickedFileds = new List<string>();
            }

            listViewPart.VisableTo = viewModel.VisableTo;
            listViewPart.As<TitlePart>().Title = viewModel.DisplayName;
            listViewPart.IsDefault = viewModel.IsDefault;
            queryPart.Name = "Query for Public View";

            projectionPart.Record.ItemsPerPage = viewModel.PageRowCount;
            //Post Selected Fields
            var layoutRecord = projectionPart.Record.LayoutRecord;
            layoutRecord.Properties.Clear();

            var category = viewModel.ItemContentType + "ContentFields";
            const string settingName = "CoeveryTextFieldSettings.IsDispalyField";
            try {
                UpdateLayoutProperties(viewModel.ItemContentType, ref layoutRecord, category, settingName, pickedFileds);
            }
            catch (Exception exception) {
                Services.Notifier.Add(NotifyType.Error,T(exception.Message));
            }
            layoutRecord.State = GetLayoutState(queryPart.Id, layoutRecord.Properties.Count, layoutRecord.Description);
            // sort
            queryPart.SortCriteria.Clear();
            if (!string.IsNullOrEmpty(viewModel.SortedBy)) {
                var sortCriterionRecord = new SortCriterionRecord {
                    Category = category,
                    Type = viewModel.SortedBy,
                    Position = queryPart.SortCriteria.Count,
                    State = GetSortState(viewModel.SortedBy, viewModel.SortMode),
                    Description = viewModel.SortedBy + " " + viewModel.SortMode
                };
                queryPart.SortCriteria.Add(sortCriterionRecord);
            }
            return listViewPart.Id;
        }

        private void UpdateLayoutProperties(string entityName, ref LayoutRecord layout, string category, string settingName, IEnumerable<string> pickedFileds) {
            var allFields = _contentDefinitionManager.GetPartDefinition(entityName).Fields.ToList();
            const string fieldTypeFormat = "{0}.{1}.";
            foreach (var property in pickedFileds) {
                var names = property.Split('.');
                var propertyMatch = string.Format(fieldTypeFormat, names[0], names[1]);
                var field = allFields.FirstOrDefault(c =>
                    string.Format(fieldTypeFormat, entityName, c.Name) == propertyMatch);
                if (field == null) {
                    continue;
                }
                var fieldStateProvider = _fieldToPropertyStateProviders.FirstOrDefault(provider=>provider.CanHandle(field.FieldDefinition.Name));
                if (fieldStateProvider == null) {
                    throw new NotSupportedException("The field type \""+ field.FieldDefinition.Name + "\" is not supported!");
                }
                var propertyRecord = new PropertyRecord {
                    Category = category,
                    Type = property,
                    Description = field.DisplayName,
                    Position = layout.Properties.Count,
                    State = fieldStateProvider.GetPropertyState(field.FieldDefinition.Name, property, field.Settings),
                    LinkToContent = field.Settings.ContainsKey(settingName) && bool.Parse(field.Settings[settingName])
                };
                layout.Properties.Add(propertyRecord);
            }
        }

        private static string GetContentTypeFilterState(string entityType) {
            const string format = @"<Form><Description></Description><ContentTypes>{0}</ContentTypes></Form>";
            return string.Format(format, entityType);
        }

        private static string GetSortState(string description, string sortMode) {
            const string format = @"<Form><Description>{0}</Description><Sort>{1}</Sort></Form>";
            return string.Format(format, description, sortMode == "Desc" ? "true" : "false");
        }

        private static string GetLayoutState(int queryId, int columnCount, string desc) {
            var datas = new Dictionary<string, string> {
                {"QueryId", queryId.ToString(CultureInfo.InvariantCulture)},
                {"Category", "Html"},
                {"Type", "ngGrid"},
                {"Description", desc},
                {"Display", "1"},
                {"DisplayType", "Summary"},
                {"Alignment", "horizontal"},
                {"Columns", columnCount.ToString(CultureInfo.InvariantCulture)},
                {"GridId", string.Empty},
                {"GridClass", string.Empty},
                {"RowClass", string.Empty}
            };

            var re = FormParametersHelper.ToString(datas);
            return re;
        }
    }
}