
using Autofac;
using Coevery.Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {
    public class FieldTypeEditorModule: Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterAdapter<IFieldTypeEditor, IConcreteFieldTypeEditor>(editor => new FieldTypeEditorAdaptor(editor));
        }
    }
}