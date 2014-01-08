using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.Projections.Models;
using Coevery.Query.Services;

namespace Coevery.Query.Controllers {
    public class TestController : Controller {
        private readonly IQueryManager _queryManager;
        private readonly IContentManager _contentManager;

        public TestController(IQueryManager queryManager, IContentManager contentManager) {
            _queryManager = queryManager;
            _contentManager = contentManager;
        }

        public void Index() {
            string typeName = "Lead";

            var part = _contentManager.Query<ListViewPart>("ListViewPage")
                .ForVersion(VersionOptions.Latest).List()
                .FirstOrDefault(item => item.IsDefault && item.ItemContentType == typeName);

            if (part == null) {
                return;
            }

            var id = part.As<ProjectionPart>().Record.QueryPartRecord.Id;
            var items = _queryManager.GetContentItems(id, typeName).ToList();
        }
    }
}