using System.Collections.Generic;
using System.Reflection.Emit;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;

namespace Coevery.Fields.DynamicTypeGeneration {
    public class TextFieldGenerationProvider : FieldGenerationProvider<TextField> {
        protected override void GenerateProperty(TypeBuilder typeBuilder, ContentPartDefinition partDefinition, ContentPartFieldDefinition partFieldDefinition, Dictionary<string, TypeBuilder> typeBuilders) {
            DefineProperty(typeBuilder, partFieldDefinition.Name, typeof(string));
        }
    }
}