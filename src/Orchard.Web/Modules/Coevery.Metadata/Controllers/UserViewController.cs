using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Projections.Models;
using System.Linq;

namespace Coevery.Metadata.Controllers
{
    public class UserViewController : ApiController
    {
        private readonly IContentManager _contentManager;

        public UserViewController(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public IEnumerable<JObject> Get()
        {
            List<JObject> re = new List<JObject>();
            var projections = _contentManager.Query<ProjectionPart>().List();
            foreach (var projectionPart in projections)
            {
                string displayName = _contentManager.Get(projectionPart.Record.QueryPartRecord.Id).As<TitlePart>().Title;
                JObject reObJ = new JObject();
                reObJ["ContentId"] = projectionPart.Id;
                reObJ["EntityType"] = projectionPart.As<TitlePart>().Title;
                reObJ["DisplayName"] = displayName;
                re.Add(reObJ);
            }
            return re;
        }

        public IEnumerable<JObject> Get(string id)
        {
            List<JObject> re = new List<JObject>();
            var projections = _contentManager.Query<ProjectionPart>().List().Where(t => t.As<TitlePart>().Title == id);
            foreach (var projectionPart in projections)
            {
                string displayName = _contentManager.Get(projectionPart.Record.QueryPartRecord.Id).As<TitlePart>().Title;
                JObject reObJ = new JObject();
                reObJ["ContentId"] = projectionPart.Id;
                reObJ["EntityType"] = projectionPart.As<TitlePart>().Title;
                reObJ["DisplayName"] = displayName;
                re.Add(reObJ);
            }
            return re;
        }

        public void Delete(int id)
        {
            var contentItem = _contentManager.Get(id);
            _contentManager.Remove(contentItem);
        }
    }
}