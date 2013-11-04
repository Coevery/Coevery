using Autofac;
using Coevery.Relationship.Services;
using Module = Autofac.Module;

namespace Coevery.Relationship.Data {
    public class RelationshipModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterGeneric(typeof(DynamicPrimaryService<,,,,>)).As(typeof(IDynamicPrimaryService<,,,,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(DynamicRelatedService<,,,,>)).As(typeof(IDynamicRelatedService<,,,,>)).InstancePerDependency();
        }
    }
}