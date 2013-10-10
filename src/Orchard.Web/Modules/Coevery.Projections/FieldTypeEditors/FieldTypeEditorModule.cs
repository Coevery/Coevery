using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {
    public class FieldTypeEditorModule: Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterAdapter<IFieldTypeEditor, IConcreteFieldTypeEditor>(editor => new FieldTypeEditorAdaptor(editor));
        }
    }
}