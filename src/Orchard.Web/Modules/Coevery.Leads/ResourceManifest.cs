using Orchard.UI.Resources;

namespace Coevery.Leads
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineStyle("ngGrid").SetUrl("ng-grid.css");
            manifest.DefineStyle("LeadMain").SetUrl("todo.css");
            manifest.DefineStyle("pnotify").SetUrl("jquery.pnotify.css");

            manifest.DefineScript("angular").SetUrl("angular.js");
            manifest.DefineScript("angularResource").SetUrl("angular-resource.js");
            manifest.DefineScript("ngGrid").SetUrl("ng-grid-2.0.0.debug.js");
            manifest.DefineScript("LeadMain").SetUrl("app/main.js");
            manifest.DefineScript("LeadLogger").SetUrl("app/logger.js");
            manifest.DefineScript("LeadContext").SetUrl("app/context.js");
            manifest.DefineScript("LeadController").SetUrl("app/controller.js");
            manifest.DefineScript("LeadDetailController").SetUrl("app/detailcontroller.js");
            manifest.DefineScript("pnotify").SetUrl("jquery.pnotify.min.js");
        }
    }
}
