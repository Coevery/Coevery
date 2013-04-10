using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Projections.Models;

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
                JObject reObJ = new JObject();
                reObJ["ContentId"] = projectionPart.Id;
                reObJ["Name"] = "--";
                reObJ["DisplayName"] = projectionPart.As<TitlePart>().Title;
                re.Add(reObJ);
            }
            return re;
        }

        public IEnumerable<JObject> Get(string id)
        {
            List<JObject> re = new List<JObject>();
             var projections = _contentManager.Query<ProjectionPart>().List();
            foreach (var projectionPart in projections)
            {
               JObject reObJ = new JObject();
                reObJ["ContentId"] = projectionPart.Id;
                reObJ["Name"] = "--";
                reObJ["DisplayName"] = projectionPart.As<TitlePart>().Title;
                re.Add(reObJ);
            }
            return re;
        }
    }
}