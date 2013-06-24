using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.ClientRoute;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Perspectives.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder)
        {
            builder.Create("PerspectiveList",
                           Feature,
                           route => route
                                        .Url("/Perspectives")
                                        .TemplateUrl("'SystemAdmin/Perspectives/List'")
                                        .Controller("PerspectiveListCtrl")
                                        .Dependencies("controllers/listcontroller"));

            builder.Create("PerspectiveCreate",
                           Feature,
                           route => route
                                        .Url("/Perspectives/Create")
                                        .TemplateUrl("'SystemAdmin/Perspectives/Create'")
                                        .Controller("PerspectiveDetailCtrl")
                                        .Dependencies("controllers/editcontroller"));

            builder.Create("PerspectiveEdit",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Perspectives/Edit/' + params.Id;}")
                                        .Controller("PerspectiveDetailCtrl")
                                        .Dependencies("controllers/detailcontroller")
                );

            builder.Create("EditNavigationItem",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/{NId:[0-9a-zA-Z]+}")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Perspectives/EditNavigationItem/' + params.NId;}")
                                        .Controller("NavigationItemDetailCtrl")
                                        .Dependencies("controllers/navigationitemdetailcontroller")
                );

            builder.Create("CreateNavigationItem",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/Create")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Perspectives/CreateNavigationItem/' + params.Id;}")
                                        .Controller("NavigationItemCreateCtrl")
                                        .Dependencies("controllers/navigationitemcreatecontroller")
                );

        }
    }
}