using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Entities.Services {
    public interface IFieldGenerationProvider : IDependency {
        void GenerateProperty(TypeBuilder typeBuilder, ContentPartDefinition partDefinition, ContentPartFieldDefinition partFieldDefinition, Dictionary<string, TypeBuilder> typeBuilders);
        string GenerateColumnName(string fieldName, string fieldTypeName);
    }

    public abstract class FieldGenerationProvider<TField> : IFieldGenerationProvider where TField : ContentField {
        protected readonly string _fieldTypeName;

        protected FieldGenerationProvider() {
            _fieldTypeName = typeof(TField).Name;
        }

        void IFieldGenerationProvider.GenerateProperty(TypeBuilder typeBuilder, ContentPartDefinition partDefinition, ContentPartFieldDefinition partFieldDefinition, Dictionary<string, TypeBuilder> typeBuilders) {
            if (_fieldTypeName == partFieldDefinition.FieldDefinition.Name) {
                GenerateProperty(typeBuilder, partDefinition, partFieldDefinition, typeBuilders);
            }
        }

        string IFieldGenerationProvider.GenerateColumnName(string fieldName, string fieldTypeName) {
            if (_fieldTypeName != fieldTypeName) {
                return null;
            }
            return GenerateColumnName(fieldName);
        }

        protected abstract void GenerateProperty(TypeBuilder typeBuilder, ContentPartDefinition partDefinition, ContentPartFieldDefinition partFieldDefinition, Dictionary<string, TypeBuilder> typeBuilders);

        protected virtual string GenerateColumnName(string fieldName) {
            return fieldName;
        }

        protected void DefineProperty(TypeBuilder typeBuilder, string fieldName, Type fieldType) {
            var fieldBuilder = typeBuilder.DefineField(
                "_" + fieldName, fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);

            var propBuilder = typeBuilder.DefineProperty(
                fieldName, PropertyAttributes.HasDefault, fieldType, Type.EmptyTypes);

            // Build Get prop
            var getMethBuilder = typeBuilder.DefineMethod(
                "get_" + fieldName, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                fieldType, Type.EmptyTypes);

            var generator = getMethBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0); // load 'this'
            generator.Emit(OpCodes.Ldfld, fieldBuilder); // load the field
            generator.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getMethBuilder);

            // Build Set prop
            var setMethBuilder = typeBuilder.DefineMethod(
                "set_" + fieldName, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void), new[] {fieldType});

            generator = setMethBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0); // load 'this'
            generator.Emit(OpCodes.Ldarg_1); // load value
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            propBuilder.SetSetMethod(setMethBuilder);
        }
    }
}