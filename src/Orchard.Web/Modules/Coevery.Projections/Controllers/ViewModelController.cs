using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Utility.Extensions;

namespace Coevery.Projections.Controllers {
    public class ViewModelController : ApiController {
        private readonly IProjectionService _projectionService;

         public ViewModelController(IProjectionService projectionService) {
             _projectionService = projectionService;
         }

        public IEnumerable<JObject> Get(int id)
        {
            ProjectionEditViewModel viewModel = _projectionService.GetProjectionViewModel(id);
            List<JObject> re = new List<JObject>();
             viewModel.LayoutViewModel.Properties.Select(c=>c.DisplayText).ToList().ForEach(c => {
                 JObject reObJ = new JObject();
                 reObJ["FieldName"] = c;
                 re.Add(reObJ);
             });
            return re;
        }

    }
}