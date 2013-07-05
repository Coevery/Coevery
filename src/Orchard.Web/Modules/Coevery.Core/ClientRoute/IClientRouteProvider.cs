using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public interface IClientRouteProvider : IDependency {
        void Discover(ClientRouteTableBuilder builder);
    }

    //public abstract class ClientRouteProviderBase : IClientRouteProvider {

    //    private readonly string _prefix;

    //    protected ClientRouteProviderBase(string prefix) {
    //        _prefix = prefix;
    //    }

    //    public void Discover(ClientRouteTableBuilder builder) {
    //        builder.Describe(_prefix + "List")
    //               .Configure(descriptor => {
    //                   descriptor.UseDefaultUrl = true;
    //                   descriptor.TemplateUrl = "'SystemAdmin/{0}/List'";
    //                   descriptor.Controller = _prefix + "ListCtrl";
    //                   descriptor.Dependencies = new[] {"controllers/listcontroller"};
    //               });

    //        builder.Describe(_prefix + "Create")
    //               .Configure(descriptor => {
    //                   descriptor.UseDefaultUrl = true;
    //                   descriptor.Url = "/Create";
    //                   descriptor.TemplateUrl = "'SystemAdmin/{0}/Create'";
    //                   descriptor.Controller = _prefix + "EditCtrl";
    //                   descriptor.Dependencies = new[] {"controllers/editcontroller"};
    //               });
    //        builder.Describe(_prefix + "Detail")
    //               .Configure(descriptor => {
    //                   descriptor.UseDefaultUrl = true;
    //                   descriptor.Url = "/{Id:[0-9a-zA-Z]+}";
    //                   descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/{0}/Detail/' + params.Id;}";
    //                   descriptor.Controller = _prefix + "DetailCtrl";
    //                   descriptor.Dependencies = new[] {"controllers/detailcontroller"};
    //               });

    //    }

    //}
}