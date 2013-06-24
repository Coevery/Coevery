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

            manifest.DefineScript("ngGrid").SetUrl("ng-grid-2.0.6.min.js", "ng-grid-2.0.6.debug.js").SetVersion("2.0.6");
            manifest.DefineStyle("ngGrid").SetUrl("ng-grid.css");

            manifest.DefineScript("pnotify").SetUrl("jquery.pnotify.min.js", "jquery.pnotify.js").SetVersion("1.2.2");
            manifest.DefineStyle("pnotify_Icons").SetUrl("jquery.pnotify.icons.css");
            manifest.DefineStyle("pnotify").SetUrl("jquery.pnotify.css").SetDependencies("pnotify_Icons");

            manifest.DefineScript("angular_UI_Router").SetUrl("angular-ui-router.min.js", "angular-ui-router.js").SetVersion("0.0.2").SetDependencies("angular");

            manifest.DefineScript("angular_couchPotato").SetUrl("angular-couchPotato.min.js", "angular-couchPotato.js").SetVersion("0.0.4").SetDependencies("angular_UI_Router");

            manifest.DefineScript("require").SetUrl("require.min.js", "require.js").SetVersion("2.1.6");

            manifest.DefineScript("i18next").SetUrl("i18next-1.6.3.min.js", "i18next-1.6.3.js").SetVersion("1.6.3");

            manifest.DefineScript("angular_detour").SetUrl("angular-detour.amd.min.js", "angular-detour.amd.js").SetVersion("0.3.1").SetDependencies("angular", "require");
        }
    }
}
