using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Coevery.Common.Events;
using Coevery.Core.Common.Attributes;
using Coevery.Core.Common.Drivers;
using Coevery.Core.Common.Handlers;
using Coevery.Core.Common.Models;
using Coevery.Core.Common.Services;
using Coevery.Relationship.Records;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.Records;
using Coevery.Data;

namespace Coevery.Relationship.Handlers {
    public class RelationshipDynamicTypeGenerationEventsHandler : IDynamicTypeGenerationEvents {
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRepository;

        public RelationshipDynamicTypeGenerationEventsHandler(
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository) {
            _manyToManyRepository = manyToManyRepository;
        }

        public void OnBuilded(ModuleBuilder moduleBuilder) {
            var records = _manyToManyRepository.Table
                .Where(x => x.Relationship.PrimaryEntity.ContentItemVersionRecord.Latest
                            && x.Relationship.RelatedEntity.ContentItemVersionRecord.Latest);
            foreach (var manyToManyRelationshipRecord in records) {
                var relationshipName = manyToManyRelationshipRecord.Relationship.Name;
                var primaryEntityName = manyToManyRelationshipRecord.Relationship.PrimaryEntity.Name;
                var relatedEntityName = manyToManyRelationshipRecord.Relationship.RelatedEntity.Name;
                var primaryName = relationshipName + primaryEntityName;
                var relatedName = relationshipName + relatedEntityName;

                var primaryPartRecordType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartRecord", "Models", primaryName),
                    moduleBuilder, typeof(ContentPartRecord))
                    .CreateType();
                var relatedPartRecordType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartRecord", "Models", relatedName),
                    moduleBuilder, typeof(ContentPartRecord))
                    .CreateType();

                var contentLinkRecordType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}ContentLinkRecord", "Models", relationshipName),
                    moduleBuilder, typeof(ContentLinkRecord))
                    .CreateType();

                var primaryPartType = BuildPartType(primaryName, relatedEntityName, primaryPartRecordType, true, moduleBuilder);
                var relatedPartType = BuildPartType(relatedName, primaryEntityName, relatedPartRecordType, false, moduleBuilder);

                BuildHandlerType(primaryName, primaryPartRecordType, moduleBuilder);
                BuildHandlerType(relatedName, relatedPartRecordType, moduleBuilder);

                BuildDriverType(primaryName, contentLinkRecordType, primaryPartType, moduleBuilder);
                BuildDriverType(relatedName, contentLinkRecordType, relatedPartType, moduleBuilder);
            }
        }

        private static TypeBuilder BuildType(string name, ModuleBuilder moduleBuilder, Type parentType) {
            var typBuilder = moduleBuilder.DefineType(name,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout, parentType);
            return typBuilder;
        }

        private static Type BuildPartType(string entityName, string otherEntityName, Type partRecordType, bool isPrimary, ModuleBuilder moduleBuilder) {
            var partBuilder = BuildType(
                string.Format("Coevery.DynamicTypes.{0}.{1}Part", "Models", entityName),
                moduleBuilder, typeof(ContentPart<>).MakeGenericType(partRecordType));

            SetRelationshipInfoAttribute(partBuilder, otherEntityName, isPrimary);
            return partBuilder.CreateType();
        }

        private static Type BuildHandlerType(string entityName, Type partRecordType, ModuleBuilder moduleBuilder) {
            var handlerType = BuildType(
                string.Format("Coevery.DynamicTypes.{0}.{1}PartHandler", "Handlers", entityName),
                moduleBuilder, typeof(DynamicContentsHandler<>).MakeGenericType(partRecordType));

            BuildHandlerCtor(handlerType, partRecordType);
            return handlerType.CreateType();
        }

        private static void BuildHandlerCtor(TypeBuilder typeBuilder, Type type) {
            Type paramType = typeof(IRepository<>);
            var paramGenericType = paramType.MakeGenericType(type);
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new Type[1] {paramGenericType});

            var generator = ctorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            Type contentType = typeof(DynamicContentsHandler<>);
            var genericContentType = contentType.MakeGenericType(type);
            var baseCtorInfo = genericContentType.GetConstructor(new Type[1] {paramGenericType});
            generator.Emit(OpCodes.Call, baseCtorInfo);
            generator.Emit(OpCodes.Ret);
        }

        private static Type BuildDriverType(string entityName, Type contentLinkRecordType, Type partType, ModuleBuilder moduleBuilder) {
            var seriviceType = typeof(IDynamicRelationshipService<>).MakeGenericType(contentLinkRecordType);
            var driverType = BuildType(
                string.Format("Coevery.DynamicTypes.{0}.{1}PartDriver", "Drivers", entityName),
                moduleBuilder,
                typeof(DynamicRelationshipPartDriver<,>).MakeGenericType(partType, contentLinkRecordType));

            BuildDriverCtor(driverType, seriviceType, contentLinkRecordType);
            return driverType.CreateType();
        }

        private static void BuildDriverCtor(TypeBuilder typeBuilder, Type seriviceType, Type contentLinkRecordType) {
            var repositoryType = typeof(IRepository<>).MakeGenericType(contentLinkRecordType);
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new Type[] {seriviceType, typeof(IContentManager), repositoryType, typeof(IContentDefinitionManager)});

            var generator = ctorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Ldarg_3);
            generator.Emit(OpCodes.Ldarg_S, (byte) 4);
            var baseCtorInfo = typeBuilder.BaseType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] {seriviceType, typeof(IContentManager), repositoryType, typeof(IContentDefinitionManager)},
                null);
            generator.Emit(OpCodes.Call, baseCtorInfo);
            generator.Emit(OpCodes.Ret);
        }

        private static void SetRelationshipInfoAttribute(TypeBuilder typeBuilder, string entityName, bool isPrimary) {
            var ctorParams = new[] {typeof(string), typeof(bool)};
            var ctorInfo = typeof(RelationshipInfoAttribute).GetConstructor(ctorParams);
            var builder = new CustomAttributeBuilder(ctorInfo, new object[] {entityName, isPrimary});

            typeBuilder.SetCustomAttribute(builder);
        }
    }
}