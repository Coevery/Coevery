using System;
using System.Web.Hosting;

namespace Coevery.Specs.Hosting {
    public class WebHostAgent : MarshalByRefObject {
        public SerializableDelegate<Action> Execute(SerializableDelegate<Action> shuttle) {
            shuttle.Delegate();
            return shuttle;
        }

        public void Shutdown() {
            HostingEnvironment.InitiateShutdown();
        }
    }
}