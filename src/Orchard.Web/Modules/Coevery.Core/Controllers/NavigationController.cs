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
            IEnumerable<MenuItem> menuItems = _navigationManager.BuildMenu(menuName).Where(item=>!item.LocalNav);
            MenuItem menuItem;
            if (id == 0)
                menuItem = menuItems.Where(item => !item.LocalNav).Count() == 0 ? null : menuItems.Where(item => !item.LocalNav).First();
            else
            {
                var tempItems = menuItems.Where(item => _menuService.GetMenu(item.Text.Text).Id == id && !item.LocalNav);
                menuItem = tempItems.Count() == 0 ? null : tempItems.First();
            }

            foreach (MenuItem m in menuItems)
                ChangeHref(m, "/OrchardLocal/Coevery#");
            ChangeHref(menuItem, "/OrchardLocal/Coevery#");

            var returnResult = new { alls = menuItems, curr = menuItem };
            var json = JsonConvert.SerializeObject(returnResult);
            
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            return message;
        }

        private void ChangeHref(MenuItem menuItem,string hrefstr)
        {
            if (menuItem==null) return;
            var menuContent = _menuService.GetMenu(menuItem.Text.Text);
            hrefstr += "/";
            hrefstr += menuContent != null ? menuContent.Id.ToString() : menuItem .Text.Text;
            menuItem.Href = hrefstr;
            if (menuItem.Items.Count() == 0)
                return;
            foreach (MenuItem _menuItem in menuItem.Items)
            {
               
                ChangeHref(_menuItem, hrefstr);
            }
        }
    }
}
