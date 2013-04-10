using Orchard.UI.Resources;

namespace Orchard.AngularJS
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("angular").SetUrl("angular.min.js", "angular.js").SetVersion("1.0.5")
                    .SetCdn("//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular.min.js", "//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular.js", true);

            manifest.DefineScript("angularResource").SetUrl("angular-resource.min.js", "angular-resource.js").SetVersion("1.0.5")
                    .SetCdn("//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular-resource.min.js", "//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular-resource.js", true);

            manifest.DefineScript("ngGrid").SetUrl("ng-grid.min.js", "ng-grid.debug.js").SetVersion("2.0.2");
            manifest.DefineStyle("ngGrid").SetUrl("ng-grid.css");

            manifest.DefineScript("pnotify").SetUrl("jquery.pnotify.min.js", "jquery.pnotify.js").SetVersion("1.2.2");
            manifest.DefineStyle("pnotify_Icons").SetUrl("jquery.pnotify.icons.css");
            manifest.DefineStyle("pnotify").SetUrl("jquery.pnotify.css").SetDependencies("pnotify_Icons");

            manifest.DefineScript("angular_UI_States").SetUrl("angular-ui-states.min.js", "angular-ui-states.js").SetVersion("0.0.1");

            manifest.DefineScript("angular_Localize").SetUrl("localize.js").SetDependencies("angular");
        }
    }
}
