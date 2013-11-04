using System.Collections.Generic;
using System.Web.Routing;
using Coevery.Environment.Extensions.Models;
using Coevery.Events;

namespace Coevery.Packaging.Events {
    public interface IExtensionDisplayEventHandler : IEventHandler {
        /// <summary>
        /// Called before an extension is displayed
        /// </summary>
        IEnumerable<string> Displaying(ExtensionDescriptor extensionDescriptor, RequestContext requestContext);
    }
}