using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Entities.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {

        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder)
        {
            builder.Create("EntityList",
                           Feature,
                           route => route
                                        .Url("/Entities")
                                        .TemplateUrl("'SystemAdmin/Entities/List'")
                                        .Controller("EntityListCtrl")
                                        .Dependencies("controllers/listcontroller"));

            builder.Create("EntityCreate",
                           Feature,
                           route => route
                                        .Url("/Entities/Create")
                                        .TemplateUrl("'SystemAdmin/Entities/Create'")
                                        .Controller("EntityEditCtrl")
                                        .Dependencies("controllers/editcontroller"));

            builder.Create("EntityEdit",
                           Feature,
                           route => route
                                        .Url("/Entities/{Id:[0-9a-zA-Z]+}/Edit")
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("EntityEditCtrl")
                                        .Dependencies("controllers/editcontroller"));

            builder.Create("EntityDetail",
                           Feature,
                           route => route
                                        .Url("/Entities/{Id:[0-9a-zA-Z]+}")
                                        .Abstract(true)
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("EntityDetailCtrl")
                                        .Dependencies("controllers/detailcontroller")
                                        .Children("Fields",
                                                  child => child
                                                               .Url(string.Empty)
                                                               .TemplateUrl("'SystemAdmin/Entities/Fields'")
                                                               .Controller("FieldsCtrl")
                                                               .Dependencies("controllers/fieldscontroller"))
                                        .Children("Relationships",
                                                  child => child
                                                               .Url("/Relationships")
                                                               .TemplateUrl("'SystemAdmin/Entities/Relationships'")
                                                               .Controller("RelationshipsCtrl")
                                                               .Dependencies("controllers/relationshipscontroller")));

        }
    }
}