using Coevery.Mvc.ClientRoute;

namespace Coevery.Modules.Services
{
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("Modules")
                .Configure(descriptor => {
                    descriptor.Abstract = true;
                    descriptor.Url = "";
                });

            builder.Describe("Modules.Features")
                .Configure(descriptor => {
                    descriptor.Url = "/Modules/Features";
                })
                .View(view => {
                    view.Name = "@";
                    view.TemplateUrl = "'" + ModuleBasePath + @"Features'";
                    view.Controller = "FeatureListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/featurelistcontroller");
                });

            builder.Describe("Modules.Installed")
             .Configure(descriptor => {
                 descriptor.Url = "/Modules/Installed";
             })
             .View(view => {
                 view.Name = "@";
             });

            builder.Describe("Modules.Recipes")
             .Configure(descriptor => {
                 descriptor.Url = "/Modules/Recipes";
             })
             .View(view => {
                 view.Name = "@";
             });
        }
    }
}