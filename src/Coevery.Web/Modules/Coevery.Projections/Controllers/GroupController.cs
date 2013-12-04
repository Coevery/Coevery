using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Data;
using Coevery.DisplayManagement;
using Coevery.Forms.Services;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Coevery.Security;
using Coevery.UI.Admin;
using Coevery.UI.Notify;

namespace Coevery.Projections.Controllers {
    [ValidateInput(false), Admin]
    public class GroupController : Controller {
        public ICoeveryServices Services { get; set; }
        private readonly IProjectionManager _projectionManager;
        private readonly IRepository<LayoutGroupRecord> _repository;
        private readonly IRepository<LayoutRecord> _layoutRepository;
        private readonly IGroupService _groupService;

        public GroupController(
            ICoeveryServices services,
            IShapeFactory shapeFactory,
            IProjectionManager projectionManager,
            IRepository<LayoutGroupRecord> repository,
            IRepository<LayoutRecord> layoutRepository,
            IGroupService groupService) {
            Services = services;
            _projectionManager = projectionManager;
            _repository = repository;
            _layoutRepository = layoutRepository;
            _groupService = groupService;
            Shape = shapeFactory;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Add(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            LayoutRecord layoutRecord = _layoutRepository.Get(id);

            if (layoutRecord == null)
            {
                return HttpNotFound();
            }
            var viewModel = new GroupAddViewModel { Id = id};
            #region Load Fields

            var fieldEntries = new List<PropertyEntry>();
            var allFields = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors);

            foreach (var field in layoutRecord.Properties)
            {
                var fieldCategory = field.Category;
                var fieldType = field.Type;

                var f = allFields.FirstOrDefault(x => fieldCategory == x.Category && fieldType == x.Type);
                if (f != null)
                {
                    fieldEntries.Add(
                        new PropertyEntry
                        {
                            Category = f.Category,
                            Type = f.Type,
                            PropertyRecordId = field.Id,
                            DisplayText = String.IsNullOrWhiteSpace(field.Description) ? f.Display(new PropertyContext { State = FormParametersHelper.ToDynamic(field.State) }).Text : field.Description,
                            Position = field.Position
                        });
                }
            }

            viewModel.Properties = fieldEntries.OrderBy(f => f.Position);

            #endregion

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id, int groupId) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var group = _repository.Get(groupId);
            if (group == null) {
                return HttpNotFound();
            }

            group.LayoutRecord.Groups.Remove(group);
            _repository.Delete(group);

            Services.Notifier.Information(T("Group Property deleted"));

            return RedirectToAction("Edit", "Layout", new {id});
        }

        public ActionResult Edit(int id, int propertyId, int groupId = -1) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var layout = _layoutRepository.Get(id);

            var propertyRecord = layout.Properties.FirstOrDefault(f => f.Id == propertyId);
            if (propertyRecord == null) {
                return HttpNotFound();
            }

            var property = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == propertyRecord.Category && x.Type == propertyRecord.Type);

            if (property == null) {
                return HttpNotFound();
            }

            var viewModel = new GroupEditViewModel {
                Id = id,
                Property = property
            };

            if (groupId != -1) {
                var groupRecord = _repository.Get(groupId);
                if (groupRecord != null) {
                    viewModel.Sort = groupRecord.Sort;
                }
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(int id, int propertyId, [DefaultValue(-1)] int groupId) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var layout = _layoutRepository.Get(id);
            var propertyRecord = layout.Properties.FirstOrDefault(f => f.Id == propertyId);

            var model = new GroupEditViewModel();
            TryUpdateModel(model);

            // validating form values
            if (ModelState.IsValid) {
                var groupRecord = layout.Groups.FirstOrDefault(g => g.Id == groupId);

                // add new property record if it's a newly created property
                if (groupRecord == null) {
                    groupRecord = new LayoutGroupRecord {
                        GroupProperty = propertyRecord,
                        Sort = model.Sort,
                        Position = layout.Groups.Count
                    };
                    layout.Groups.Add(groupRecord);
                }

                return RedirectToAction("Edit", "Layout", new {id});
            }

            // model is invalid, display it again
            var property = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == propertyRecord.Category && x.Type == propertyRecord.Type);

            var viewModel = new GroupEditViewModel {Id = id, Property = property, Sort = model.Sort};

            return View(viewModel);
        }

        public ActionResult Move(string direction, int id, int layoutId) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            switch (direction) {
                case "up":
                    _groupService.MoveUp(id);
                    break;
                case "down":
                    _groupService.MoveDown(id);
                    break;
                default:
                    throw new ArgumentException("direction");
            }

            return RedirectToAction("Edit", "Layout", new {id = layoutId});
        }
    }
}