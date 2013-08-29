using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Projections.ViewModels;
using Orchard.Settings;
using Orchard.UI.Notify;
using Orchard.Utility;

namespace Coevery.Projections.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IOrchardServices _services;
        private readonly ISiteService _siteService;
        private readonly IQueryService _queryService;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly IFormManager _formManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IRepository<LayoutRecord> _layoutRepository;
        private readonly ILayoutPropertyService _layoutPropertyService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public ProjectionService(
             IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IQueryService queryService,
            IProjectionManager projectionManager,
            IContentManager contentManager,
            IFormManager formManager,
            ITransactionManager transactionManager,
            IRepository<LayoutRecord> layoutRepository,
            ILayoutPropertyService layoutPropertyService,
            IContentDefinitionManager contentDefinitionManager)
        {
            _services = services;
            _siteService = siteService;
            _queryService = queryService;
            _projectionManager = projectionManager;
            _contentManager = contentManager;
            _formManager = formManager;
            _transactionManager = transactionManager;
            _layoutRepository = layoutRepository;
            Services = services;
            _layoutPropertyService = layoutPropertyService;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ProjectionEditViewModel GetTempProjection(string entityType) {
            var viewModel = new ProjectionEditViewModel {Name = entityType, DisplayName = string.Empty};

            //Get all field
            var contentPart = _contentDefinitionManager.GetPartDefinition(entityType);
            viewModel.AllFields = contentPart == null ? new BindingList<ContentPartFieldDefinition>() : contentPart.Fields;
            return viewModel;
        }

        public ProjectionEditViewModel CreateTempProjection(string entityType)
        {
            var viewModel = new ProjectionEditViewModel {Name = string.Empty, DisplayName = string.Empty};

            //Create Projection&Query
            var projectionItem = _contentManager.New("ProjectionPage");
            var queryItem = _contentManager.New("Query");
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPart = queryItem.As<QueryPart>();
            // projectionPart.Record.QueryPartRecord = queryPart.Record;

            _contentManager.Create(projectionItem, VersionOptions.Draft);
            _contentManager.Create(queryItem, VersionOptions.Draft);
            projectionPart.Record.QueryPartRecord = queryPart.Record;
            projectionPart.As<TitlePart>().Title = entityType;
            queryPart.As<TitlePart>().Title = entityType;
            _contentManager.Publish(queryItem);
            _contentManager.Publish(projectionItem);

            var queryViewModel = this.GetQueryViewModel(queryPart);
            viewModel.QueryViewModel = queryViewModel;
            viewModel.Id = projectionItem.Id;

            //Create ng-grid layout for query
            var layoutViewModel = this.CreateTempLayout(queryPart, "", "ngGrid");
            viewModel.LayoutViewModel = layoutViewModel;
            if (layoutViewModel != null) projectionPart.Record.LayoutRecord = queryPart.Layouts[0];

            //Create filter  for query
            var filterGroup = new FilterGroupRecord();
            queryPart.Record.FilterGroups.Clear();
            queryPart.Record.FilterGroups.Add(filterGroup);
            var filterRecord = new FilterRecord
            {
                Category = "Content",
                Type = "ContentTypes",
                Position = filterGroup.Filters.Count,
                State = this.GetContentTypeFilterState(entityType)
            };
            filterGroup.Filters.Add(filterRecord);

            //Get all field
            var contentPart = _contentDefinitionManager.GetPartDefinition(entityType);
            viewModel.AllFields = contentPart == null ? new BindingList<ContentPartFieldDefinition>() : contentPart.Fields;
            return viewModel;
        }


        public ProjectionEditViewModel GetProjectionViewModel(int id)
        {
            var viewModel = new ProjectionEditViewModel();
            //Get Projection&QueryPart
            var projectionItem = _contentManager.Get(id, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryId = projectionPart.Record.QueryPartRecord.Id;
            var queryItem = _contentManager.Get(queryId, VersionOptions.Latest);
            var queryPart = queryItem.As<QueryPart>();
            viewModel.Id = id;
            viewModel.Name = projectionItem.As<TitlePart>().Title;
            viewModel.DisplayName = queryPart.Name;
            viewModel.QueryViewModel = this.GetQueryViewModel(queryPart);

            //Get LayoutViewModel;
            LayoutRecord layoutRecord = projectionPart.Record.LayoutRecord;
            viewModel.LayoutViewModel = this.GetLayoutEditViewModel(layoutRecord);

            //Get AllFields
            var contentPart = _contentDefinitionManager.GetPartDefinition(viewModel.Name);
            viewModel.AllFields = contentPart == null ? new BindingList<ContentPartFieldDefinition>() : contentPart.Fields;

            var sortCriteria = queryPart.SortCriteria.FirstOrDefault();
            if (sortCriteria != null)
            {
                viewModel.SortedBy = sortCriteria.Type;
                Regex regex = new Regex("<Sort>(.+?)</Sort>");
                var matche = regex.Match(sortCriteria.State);
                viewModel.SortMode = matche.Value.Contains("true") ? "Desc" : "Asc";
            }

            var layoutPropertyPart = _layoutPropertyService.GetLayoutPropertyByQueryid(queryPart.Id);

            if (layoutPropertyPart != null)
            {
                viewModel.VisableTo = layoutPropertyPart.VisableTo;
                viewModel.PageRowCount = layoutPropertyPart.PageRowCount;
            }
            return viewModel;
        }

        public bool EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFileds) {
            if (pickedFileds == null) pickedFileds = new List<string>();
            var projectionPart = _contentManager.Get<ProjectionPart>(id, VersionOptions.Latest);
            var queryId = projectionPart.Record.QueryPartRecord.Id;
            var queryPart = _contentManager.Get<QueryPart>(queryId, VersionOptions.Latest);

            //Post DisplayName
            queryPart.As<TitlePart>().Title = viewModel.DisplayName;
            
            //Post Selected Fields
            LayoutRecord layoutRecord = projectionPart.Record.LayoutRecord;
            layoutRecord.Properties.Clear();

            var allFields = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors);
            string category = viewModel.Name + "ContentFields";
            var fields = allFields.Where(c => c.Category == category).ToList();
            foreach (var property in pickedFileds) {
                var field = allFields.FirstOrDefault(c => c.Category == category && c.Type.StartsWith(viewModel.Name + "." + property));
                if (field == null) {
                    throw new Exception("selected field not found:" + property);
                }
                var propertyRecord = new PropertyRecord {
                    Category = category,
                    Type = field.Type,
                    Description = field.Name.Text.Split(':').Length > 0 ? field.Name.Text.Split(':')[0] : string.Empty,
                    Position = layoutRecord.Properties.Count,
                    State = GetPropertyState(property),
                    LinkToContent = layoutRecord.Properties.Count == 0
                };
                layoutRecord.Properties.Add(propertyRecord);
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

            // VisableTo and pageRowCount
            var layoutPropertyRecord = new LayoutPropertyRecord {
                VisableTo = viewModel.VisableTo,
                PageRowCount = viewModel.PageRowCount,
                QueryPartRecord_id = queryPart.Id
            };
            _layoutPropertyService.CreateLayoutProperty(layoutPropertyRecord);
            return true;
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

        private string GetSortState(string description, string sortMode)
        {
            string format = @"<Form><Description>{0}</Description><Sort>{1}</Sort></Form>";
            return string.Format(format, description, sortMode == "Desc" ? "true" : "false");
        }

        private string GetPropertyState(string filedName)
        {
            //Dictionary<string, string> datas = new Dictionary<string, string>();
            //datas.Add("Description",filedName);
            //var re = FormParametersHelper.ToString(datas);
            string format = @"<Form>
                  <Description>{0}</Description>
                  <LinkToContent>true</LinkToContent>
                  <ExcludeFromDisplay>false</ExcludeFromDisplay>
                  <CreateLabel>false</CreateLabel>
                  <Label></Label>
                  <CustomizePropertyHtml>false</CustomizePropertyHtml>
                  <CustomPropertyTag></CustomPropertyTag>
                  <CustomPropertyCss></CustomPropertyCss>
                  <CustomizeLabelHtml>false</CustomizeLabelHtml>
                  <CustomLabelTag></CustomLabelTag>
                  <CustomLabelCss></CustomLabelCss>
                  <CustomizeWrapperHtml>false</CustomizeWrapperHtml>
                  <CustomWrapperTag></CustomWrapperTag>
                  <CustomWrapperCss></CustomWrapperCss>
                  <NoResultText></NoResultText>
                  <ZeroIsEmpty>false</ZeroIsEmpty>
                  <HideEmpty>false</HideEmpty>
                  <RewriteOutput>false</RewriteOutput>
                  <RewriteText></RewriteText>
                  <TrimLength>false</TrimLength>
                  <MaxLength>0</MaxLength>
                  <TrimOnWordBoundary>false</TrimOnWordBoundary>
                  <AddEllipsis>false</AddEllipsis>
                  <StripHtmlTags>false</StripHtmlTags>
                  <TrimWhiteSpace>false</TrimWhiteSpace>
                  <PreserveLines>false</PreserveLines>
                  <__RequestVerificationToken>ciDAMpqWekeZ9akUdP45wrGCR-RbzW3fKdnX3IztKQZd1zVqvBQLpNHgueKedAEt6YyEZ3kGF1zFcuvCaFedP2XovGdniK5wpqg5TMcxICtLElQDw14kWyePWJx4KI4Wqw8usNPgLFlXNRSQWIcBbv3JNZ5ITLKNigxo4znvIrk1</__RequestVerificationToken>
                    </Form>";
            return string.Format(format, filedName);
        }

        private string GetLayoutState(int queryId, int columnCount, string desc) {
            var datas = new Dictionary<string, string> {
                {"QueryId", queryId.ToString()},
                {"Category", "Html"},
                {"Type", "ngGrid"},
                {"Description", desc},
                {"Display", "1"},
                {"DisplayType", "Summary"},
                {"Alignment", "horizontal"},
                {"Columns", columnCount.ToString()},
                {"GridId", string.Empty},
                {"GridClass", string.Empty},
                {"RowClass", string.Empty}
            };

            var re = FormParametersHelper.ToString(datas);
            return re;
        }

        private LayoutEditViewModel GetLayoutEditViewModel(LayoutRecord layoutRecord)
        {
            if (layoutRecord == null)
            {
                return null;
            }

            var layoutDescriptor = _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == layoutRecord.Category && x.Type == layoutRecord.Type);

            // build the form, and let external components alter it
            var form = _formManager.Build(layoutDescriptor.Form) ?? Services.New.EmptyForm();

            var viewModel = new LayoutEditViewModel
            {
                Id = layoutRecord.Id,
                QueryId = layoutRecord.QueryPartRecord.Id,
                Category = layoutDescriptor.Category,
                Type = layoutDescriptor.Type,
                Description = layoutRecord.Description,
                Display = layoutRecord.Display,
                DisplayType = String.IsNullOrWhiteSpace(layoutRecord.DisplayType) ? "Summary" : layoutRecord.DisplayType,
                Layout = layoutDescriptor,
                Form = form,
                GroupPropertyId = layoutRecord.GroupProperty == null ? 0 : layoutRecord.GroupProperty.Id
            };

            // bind form with existing values
            var parameters = FormParametersHelper.FromString(layoutRecord.State);
            _formManager.Bind(form, new DictionaryValueProvider<string>(parameters, CultureInfo.InvariantCulture));

            var fieldEntries = new List<PropertyEntry>();
            foreach (var field in layoutRecord.Properties)
            {

                fieldEntries.Add(
                    new PropertyEntry
                    {
                        Category = field.Category,
                        Type = field.Type,
                        PropertyRecordId = field.Id,
                        DisplayText = field.Description,
                        Position = field.Position
                    });

            }
            viewModel.Properties = fieldEntries.OrderBy(f => f.Position).ToList();
            return viewModel;
        }

        private LayoutEditViewModel CreateTempLayout(QueryPart query, string category, string type)
        {
            LayoutEditViewModel model = new LayoutEditViewModel();
            model.Category = category;
            model.Type = type;
            model.Layout = _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == category && x.Type == type);

            var layoutRecord = new LayoutRecord { Category = "Html", Type = type };

            // save form parameters
            layoutRecord.Description = "DefaultLayoutFor" + query.Name;
            layoutRecord.Display = 1;
            layoutRecord.DisplayType = "Summary";
            query.Layouts.Add(layoutRecord);

            Services.Notifier.Information(T("Layout Created"));
            model.QueryId = query.Id;
            model.Description = layoutRecord.Description;
            model.Display = layoutRecord.Display;
            model.DisplayType = layoutRecord.DisplayType;
            return model;
        }

        public AdminEditViewModel GetQueryViewModel(QueryPart query)
        {
            var viewModel = new AdminEditViewModel
            {
                Id = query.Id,
                Name = query.Name
            };

            #region Load Filters
            var filterGroupEntries = new List<FilterGroupEntry>();
            foreach (var group in query.FilterGroups)
            {
                var filterEntries = new List<FilterEntry>();

                foreach (var filter in group.Filters)
                {
                    filterEntries.Add(
                        new FilterEntry
                        {
                            Category = filter.Category,
                            Type = filter.Type,
                            FilterRecordId = filter.Id,
                            DisplayText = filter.Description
                        });
                }

                filterGroupEntries.Add(new FilterGroupEntry { Id = group.Id, Filters = filterEntries });
            }

            viewModel.FilterGroups = filterGroupEntries;

            #endregion

            #region Load Sort criterias
            var sortCriterionEntries = new List<SortCriterionEntry>();
            var allSortCriteria = _projectionManager.DescribeSortCriteria().SelectMany(x => x.Descriptors).ToList();

            foreach (var sortCriterion in query.SortCriteria.OrderBy(s => s.Position))
            {
                var category = sortCriterion.Category;
                var type = sortCriterion.Type;

                var f = allSortCriteria.FirstOrDefault(x => category == x.Category && type == x.Type);
                if (f != null)
                {
                    sortCriterionEntries.Add(
                        new SortCriterionEntry
                        {
                            Category = f.Category,
                            Type = f.Type,
                            SortCriterionRecordId = sortCriterion.Id,
                            DisplayText = String.IsNullOrWhiteSpace(sortCriterion.Description) ? f.Display(new SortCriterionContext { State = FormParametersHelper.ToDynamic(sortCriterion.State) }).Text : sortCriterion.Description
                        });
                }
            }

            viewModel.SortCriteria = sortCriterionEntries;

            #endregion

            #region Load Layouts
            var layoutEntries = new List<LayoutEntry>();
            var allLayouts = _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).ToList();
            foreach (var layout in query.Layouts)
            {
                var category = layout.Category;
                var type = layout.Type;

                var f = allLayouts.FirstOrDefault(x => category == x.Category && type == x.Type);
                if (f != null)
                {
                    layoutEntries.Add(
                        new LayoutEntry
                        {
                            Category = f.Category,
                            Type = f.Type,
                            LayoutRecordId = layout.Id,
                            DisplayText = String.IsNullOrWhiteSpace(layout.Description) ? f.Display(new LayoutContext { State = FormParametersHelper.ToDynamic(layout.State) }).Text : layout.Description
                        });
                }
            }

            viewModel.Layouts = layoutEntries;

            #endregion

            return viewModel;
        }


    }
}