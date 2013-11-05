using System;
using NUnit.Framework;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Aspects;
using Coevery.Core.Contents;
using Coevery.Data;
using Coevery.Roles.Models;
using Coevery.Roles.Services;
using Coevery.Security;
using Coevery.Security.Permissions;
using Coevery.Specs.Hosting.Coevery.Web;
using TechTalk.SpecFlow;
using Coevery.Localization.Services;
using System.Linq;

namespace Coevery.Specs.Bindings {
    [Binding]
    public class Settings : BindingBase {

        [When(@"I have ""(.*)"" as the default culture")]
        public void DefineDefaultCulture(string cultureName) {

            var webApp = Binding<WebAppHosting>();
            webApp.Host.Execute(() => {
                using ( var environment = MvcApplication.CreateStandaloneEnvironment("Default") ) {
                    var coeveryServices = environment.Resolve<ICoeveryServices>();
                    var cultureManager = environment.Resolve<ICultureManager>();

                    var currentCultures = cultureManager.ListCultures();
                    if (!currentCultures.Contains(cultureName)) {
                        cultureManager.AddCulture(cultureName);
                    }

                    coeveryServices.WorkContext.CurrentSite.SiteCulture = cultureName;
                }
            });
        }
    }
}
