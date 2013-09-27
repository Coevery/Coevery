using System;
using Orchard;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {

    public delegate void Filter(FilterContext context, IFieldTypeEditor fieldTypeEditor, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field);

    public interface IConcreteFieldTypeEditor : IFieldTypeEditor {
        bool CanHandle(string fieldTypeName, Type storageType);
        Filter Filter { get; set; }
    }
}