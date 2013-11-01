using System.Web;
using Coevery.Mvc;

namespace Coevery.Tests.Stubs {
    public class StubHttpContextAccessor : IHttpContextAccessor {
        private HttpContextBase _httpContext;

        public HttpContextBase StubContext {
            set { _httpContext = value; }
        }

        public HttpContextBase Current() {
            return _httpContext;
        }
    }
}