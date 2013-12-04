using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using Coevery.Common.Extensions;
using Coevery.Core.Common.ViewModels;
using Coevery.Data;
using Coevery.Entities.Services;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.Core.Title.Models;
using Coevery.Forms.Services;
using Coevery.Localization;
using Coevery.UI.Notify;

namespace Coevery.Projections.Services {
    public class ProjectionService : IProjectionService {
        public const string DefaultLayoutCategory = "Grids";
        public const string DefaultLayoutType = "Default";
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IFieldToPropertyStateProvider> _fieldToPropertyStateProviders;
        private readonly IFormManager _formManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ProjectionService(
            ICoeveryServices services,
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

        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }

        public IEnumerable<PicklistItemViewModel> GetFieldDescriptors(string entityType, int projectionId) {
            var category = entityType.ToPartName() + "ContentFields";
            var fieldDescriptors = _projectionManager.DescribeProperties()
                .Where(x => x.Category == category).SelectMany(x => x.Descriptors)
                .Select(element => new PicklistItemViewModel {
                    Text = element.Name.Text,
                    Value = element.Type
                }).ToList();
            if (projectionId <= 0)
                return fieldDescriptors;
            var selectedFields = _contentManager.Get<ProjectionPart>(projectionId).Record.LayoutRecord.Properties;
            var order = 0;
            foreach (var field in selectedFields) {
                var viewModel = fieldDescriptors.Find(model => model.Value == field.Type);
                viewModel.Selected = true;
                viewModel.Order = ++order;
            }
            return fieldDescriptors;
        }

        public ProjectionEditViewModel GetProjectionViewModel(int id) {
            var viewModel = new ProjectionEditViewModel();
            //Get Projection&QueryPart
            var projectionItem = _contentManager.Get(id, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryId = projectionPart.Record.QueryPartRecord.Id;
            var queryPart = _contentManager.Get<QueryPart>(queryId, VersionOptions.Latest);
            var layout = projectionPart.Record.LayoutRecord;
            var listViewPart = projectionItem.As<ListViewPart>();
            viewModel.Id = id;
            viewModel.ItemContentType = listViewPart.ItemContentType.ToPartName();
            viewModel.DisplayName = listViewPart.As<TitlePart>().Title;
            viewModel.VisableTo = listViewPart.VisableTo;
            viewModel.IsDefault = listViewPart.IsDefault;
            //Get AllFields
            viewModel.Fields = GetFieldDescriptors(listViewPart.ItemContentType, id);
            //Layout related
            viewModel.LayoutId = layout.Id;
            viewModel.Layout = _projectionManager.DescribeLayouts()
                    .SelectMany(descr => descr.Descriptors)
                    .FirstOrDefault(descr => descr.Category == layout.Category && descr.Type == layout.Type);
            if (viewModel.Layout == null) {
                throw new InstanceNotFoundException(T("Layout not found!").Text);
            }
            viewModel.Form = _formManager.Build(viewModel.Layout.Form) ?? Services.New.EmptyForm();
            viewModel.State = FormParametersHelper.FromString(layout.State);
            viewModel.Form.Fields = viewModel.Fields;
            viewModel.Form.State = viewModel.State;

            return viewModel;
        }

        public string UpdateViewOnEntityAltering(string entityName) {
            var entityType = _contentDefinitionManager.GetTypeDefinition(entityName);
            var listViewParts = _contentManager.Query<ListViewPart, ListViewPartRecord>()
                .Where(record => record.ItemContentType == entityName).List();
            if (entityType == null || listViewParts == null || !listViewParts.Any()) {
                return "Invalid entity name!";
            }
            var category = entityName.ToPartName() + "ContentFields";
            const string settingName = "TextFieldSettings.IsDisplayField";
            foreach (var view in listViewParts) {
                var projection = view.As<ProjectionPart>().Record;
                var layout = projection.LayoutRecord;
                var pickedFileds = (from field in layout.Properties
                                    select field.Type).ToArray();
                UpdateLayoutProperties(entityName.ToPartName(), ref layout, category, settingName, pickedFileds);
                var state = FormParametersHelper.FromString(layout.State);
                layout.State = FormParametersHelper.ToString(MergeDictionary(
                    new[] { state, GetLayoutState(projection.QueryPartRecord.Id, layout.Properties.Count, layout.Description) }));
            }
            return null;
        }

        public int EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFileds) {
            ListViewPart listViewPart;
            ProjectionPart projectionPart;
            QueryPart queryPart;
            if (id == 0) {
                listViewPart = _contentManager.New<ListViewPart>("ListViewPage");
                listViewPart.ItemContentType = viewModel.ItemContentType.RemovePartSuffix();
                queryPart = _contentManager.New<QueryPart>("Query");

                var layout = new LayoutRecord {
                    Category = viewModel.Layout.Category,
                    Type = viewModel.Layout.Type,
                    Description = viewModel.Layout.Description.Text,
                    Display = 1
                };

                queryPart.Layouts.Add(layout);
                projectionPart = listViewPart.As<ProjectionPart>();
                projectionPart.Record.LayoutRecord = layout;
                projectionPart.Record.QueryPartRecord = queryPart.Record;

                /*@todo: when layout is tree grid, need to do some extra logic*/
                var filterGroup = new FilterGroupRecord();
                queryPart.Record.FilterGroups.Add(filterGroup);
                var filterRecord = new FilterRecord {
                    Category = "Content",
                    Type = "ContentTypes",
                    Position = filterGroup.Filters.Count,
                    State = GetContentTypeFilterState(listViewPart.ItemContentType)
                };
                filterGroup.Filters.Add(filterRecord);

                _contentManager.Create(queryPart.ContentItem);
                _contentManager.Create(projectionPart.ContentItem);
            } else {
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

            //Post Selected Fields
            var layoutRecord = projectionPart.Record.LayoutRecord;

            var category = viewModel.ItemContentType + "ContentFields";
            const string settingName = "TextFieldSettings.IsDisplayField";
            try {
                UpdateLayoutProperties(viewModel.ItemContentType, ref layoutRecord, category, settingName, pickedFileds);
            }
            catch (Exception exception) {
                Services.Notifier.Add(NotifyType.Error, T(exception.Message));
            }
            layoutRecord.State = FormParametersHelper.ToString(
                MergeDictionary(new[] {
                    viewModel.State, GetLayoutState(queryPart.Id, layoutRecord.Properties.Count, layoutRecord.Description)
                }));
            if (viewModel.Layout.Category == DefaultLayoutCategory && viewModel.Layout.Type == DefaultLayoutType) {
                projectionPart.Record.ItemsPerPage = Convert.ToInt32(viewModel.State["PageRowCount"]);
                // sort
                queryPart.SortCriteria.Clear();
                if (!string.IsNullOrEmpty(viewModel.State["SortedBy"])) {
                    var sortCriterionRecord = new SortCriterionRecord {
                        Category = category,
                        Type = viewModel.State["SortedBy"],
                        Position = queryPart.SortCriteria.Count,
                        State = GetSortState(viewModel.State["SortedBy"], viewModel.State["SortMode"]),
                        Description = viewModel.State["SortedBy"] + " " + viewModel.State["SortMode"]
                    };
                    queryPart.SortCriteria.Add(sortCriterionRecord);
                }
            }
            return listViewPart.Id;
        }

        private void UpdateLayoutProperties(string partName, ref LayoutRecord layout, string category, string settingName, IEnumerable<string> pickedFileds) {
            var allFields = _contentDefinitionManager.GetPartDefinition(partName).Fields.ToList();
            const string fieldTypeFormat = "{0}.{1}.";
            layout.Properties.Clear();
            foreach (var property in pickedFileds) {
                var names = property.Split('.');
                var propertyMatch = string.Format(fieldTypeFormat, names[0], names[1]);
                var field = allFields.FirstOrDefault(c =>
                    string.Format(fieldTypeFormat, partName, c.Name) == propertyMatch);
                if (field == null) {
                    continue;
                }
                var fieldStateProvider = _fieldToPropertyStateProviders.FirstOrDefault(provider => provider.CanHandle(field.FieldDefinition.Name));
                if (fieldStateProvider == null) {
                    throw new NotSupportedException("The field type \"" + field.FieldDefinition.Name + "\" is not supported!");
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

        private static IDictionary<string, string> GetLayoutState(int queryId, int columnCount, string descr) {
            return new Dictionary<string, string> {
                {"QueryId", queryId.ToString(CultureInfo.InvariantCulture)},
                {"Description", descr},
                {"Display", "1"},
                {"DisplayType", "Summary"},
                {"Columns", columnCount.ToString(CultureInfo.InvariantCulture)},
                {"GridId", string.Empty},
                {"GridClass", string.Empty},
                {"RowClass", string.Empty}
            };
        }

        private static IDictionary<string, string> MergeDictionary(IEnumerable<IDictionary<string, string>> dictionaries, bool useLast = true) {
            return dictionaries.SelectMany(pair => pair)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => useLast ? group.Last() : group.First());
        }
    }
}