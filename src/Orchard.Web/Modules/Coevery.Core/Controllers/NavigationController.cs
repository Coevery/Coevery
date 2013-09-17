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
    public class NavigationController : ApiController
    {
        private readonly INavigationManager _navigationManager;
        private readonly IMenuManager _menuManager;
        private readonly IMenuService _menuService;
        public NavigationController(
            INavigationManager navigationManager,
            IMenuService menuService,
            IMenuManager menuManager)
        {
            _navigationManager = navigationManager;
            _menuService = menuService;
            _menuManager = menuManager;
        }
        // GET api/menues
        public HttpResponseMessage Get(int id)
        {
            if (id < 0) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            const string menuName = "FrontMenu";
            IEnumerable<MenuItem> menuItems = _navigationManager.BuildMenu(menuName);
            MenuItem menuItem;
            if (id == 0)
                menuItem = menuItems.First();
            else
            {
                var tempItems = menuItems.Where(item => _menuService.GetMenu(item.Text.Text).Id == id);
                menuItem = tempItems.Count() == 0 ? null : tempItems.First();
            }
            var returnResult = new { alls = menuItems, curr = menuItem };
            var json = JsonConvert.SerializeObject(returnResult);
            
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            return message;
        }

    }
}
