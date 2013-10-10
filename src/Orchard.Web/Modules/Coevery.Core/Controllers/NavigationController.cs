using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.Core.Navigation.Services;
using Orchard.UI.Navigation;

namespace Coevery.Core.Controllers
{
    public class NavigationController : ApiController {
        private readonly INavigationManager _navigationManager;
        private readonly IMenuService _menuService;

        public NavigationController(
            INavigationManager navigationManager,
            IMenuService menuService) {
            _navigationManager = navigationManager;
            _menuService = menuService;
        }

        // GET api/menues
        public HttpResponseMessage Get() {
            const string menuName = "FrontMenu";
            var menuItems = _navigationManager.BuildMenu(menuName).Where(item => !item.LocalNav).ToList();

            var json = JsonConvert.SerializeObject(menuItems);

            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};
            return message;
        }
    }
}
