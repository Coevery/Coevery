using Coevery.Mvc.ClientRoute;

namespace Coevery.CRM.Convertor {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public ClientRouteProvider() {
            IsFrontEnd = true;
        }

        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("Root.DevExpress")
                .Configure(descriptor => {
                    descriptor.Url = "/DevExpress";
                    descriptor.Priority = 20;
                })
                .View(view => {
                    view.Name = "@";
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + BasePath + @"Coevery.CRM.Convertor';
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                });
        }
    }
}