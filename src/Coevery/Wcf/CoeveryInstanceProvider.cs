using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Autofac;
using Autofac.Core;

namespace Coevery.Wcf {
    public class CoeveryInstanceProvider : IInstanceProvider {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IComponentRegistration _componentRegistration;

        public CoeveryInstanceProvider(IWorkContextAccessor workContextAccessor, IComponentRegistration componentRegistration) {
            _workContextAccessor = workContextAccessor;
            _componentRegistration = componentRegistration;
        }

        public object GetInstance(InstanceContext instanceContext, Message message) {
            CoeveryInstanceContext item = new CoeveryInstanceContext(_workContextAccessor);
            instanceContext.Extensions.Add(item);
            return item.Resolve(_componentRegistration);

        }

        public object GetInstance(InstanceContext instanceContext) {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance) {
            CoeveryInstanceContext context = instanceContext.Extensions.Find<CoeveryInstanceContext>();
            if (context != null) {
                context.Dispose();
            }
        }
    }
}
