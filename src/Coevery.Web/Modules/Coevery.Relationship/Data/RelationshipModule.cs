using Autofac;
using Coevery.Core.Common.Services;
using Module = Autofac.Module;

namespace Coevery.Relationship.Data {
    public class RelationshipModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterGeneric(typeof(DynamicRelationshipService<>)).As(typeof(IDynamicRelationshipService<>)).InstancePerDependency();
        }
    }
}