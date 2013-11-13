using Coevery.Mvc.ClientRoute;

namespace Coevery.Translations.Services
{
    public class ClientRouteProvider : ClientRouteProviderBase
    {
        public override void Discover(ClientRouteTableBuilder builder)
        {
            //Index.cshtml
            builder.Describe("TranslationsList")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Index";
                })
                .View(view =>
                {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Index'";
                    view.Controller = "TranslationListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/listcontroller");
                });

            //Culture.cshtml
            builder.Describe("TranslationsCulture")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Culture/{Id}";
                })
                .View(view =>
                {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Culture/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "TranslationCultureCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/culturecontroller");
                });

            //Detail.cshtml
            builder.Describe("TranslationsDetail")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Detail/{Culture}/{Path}";
                })
                .View(view =>
                {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Detail/' + $stateParams.Culture + '/' + $stateParams.Path; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "TranslationDetailCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/detailcontroller");
                });

            //Import.cshtml
            builder.Describe("TranslationsImport")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Import";
                })
                .View(view =>
                {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Import'";
                    view.Controller = "TranslationImportCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/importcontroller");
                });


            //Export.cshtml
            builder.Describe("TranslationsExport")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Export";
                })
                .View(view =>
                {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Export'";
                    view.Controller = "TranslationExportCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/exportcontroller");
                });

            //Search.cshtml
            builder.Describe("TranslationsSearch")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/Translations/Search";
                })
                .View(view =>
                {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Search'";
                    view.Controller = "TranslationSearchCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/searchcontroller");
                });
        }
    }
}