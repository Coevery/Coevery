using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Orchard.UI.Navigation;

namespace Coevery.Core.Controllers {
    public class NavigationController : ApiController {
        private readonly INavigationManager _navigationManager;

        public NavigationController(INavigationManager navigationManager) {
            _navigationManager = navigationManager;
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