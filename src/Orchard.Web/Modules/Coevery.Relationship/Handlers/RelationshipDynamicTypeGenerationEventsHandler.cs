using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Coevery.Core.Events;
using Coevery.Core.Handlers;
using Coevery.Relationship.Drivers;
using Coevery.Relationship.Models;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Data;

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
                var primaryName = relationshipName + manyToManyRelationshipRecord.Relationship.PrimaryEntity.Name;
                var relatedName = relationshipName + manyToManyRelationshipRecord.Relationship.RelatedEntity.Name;

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
                    moduleBuilder, typeof(ContentLinkRecord<,>).MakeGenericType(primaryPartRecordType, relatedPartRecordType))
                    .CreateType();

                var primaryPartType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}Part", "Models", primaryName),
                    moduleBuilder, typeof(ContentPart<>).MakeGenericType(primaryPartRecordType))
                    .CreateType();
                var relatedPartType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}Part", "Models", relatedName),
                    moduleBuilder, typeof(ContentPart<>).MakeGenericType(relatedPartRecordType))
                    .CreateType();

                var primaryHandlerType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartHandler", "Handlers", primaryName),
                    moduleBuilder, typeof(DynamicContentsHandler<>).MakeGenericType(primaryPartRecordType));
                BuildHandlerCtor(primaryHandlerType, primaryPartRecordType);
                primaryHandlerType.CreateType();

                var relatedHandlerType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartHandler", "Handlers", relatedName),
                    moduleBuilder, typeof(DynamicContentsHandler<>).MakeGenericType(relatedPartRecordType));
                BuildHandlerCtor(relatedHandlerType, relatedPartRecordType);
                relatedHandlerType.CreateType();

                var primarySeriviceType = typeof(IDynamicPrimaryService<,,,,>).MakeGenericType(primaryPartType, relatedPartType, primaryPartRecordType, relatedPartRecordType, contentLinkRecordType);
                var primaryDriverType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartDriver", "Drivers", primaryName),
                    moduleBuilder,
                    typeof(DynamicPrimaryPartDriver<,,,,>).MakeGenericType(primaryPartType, relatedPartType, primaryPartRecordType, relatedPartRecordType, contentLinkRecordType));
                BuildDriverCtor(primaryDriverType, primarySeriviceType, contentLinkRecordType, manyToManyRelationshipRecord.Relationship.RelatedEntity.Name);
                primaryDriverType.CreateType();

                var relatedSeriviceType = typeof(IDynamicRelatedService<,,,,>).MakeGenericType(primaryPartType, relatedPartType, primaryPartRecordType, relatedPartRecordType, contentLinkRecordType);
                var relatedDriverType = BuildType(
                    string.Format("Coevery.DynamicTypes.{0}.{1}PartDriver", "Drivers", relatedName),
                    moduleBuilder,
                    typeof(DynamicRelatedPartDriver<,,,,>).MakeGenericType(primaryPartType, relatedPartType, primaryPartRecordType, relatedPartRecordType, contentLinkRecordType));
                BuildDriverCtor(relatedDriverType, relatedSeriviceType, contentLinkRecordType, manyToManyRelationshipRecord.Relationship.PrimaryEntity.Name);
                relatedDriverType.CreateType();
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

        private static void BuildDriverCtor(TypeBuilder typeBuilder, Type seriviceType, Type contentLinkRecordType, string name) {
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
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldstr, name);
            generator.Emit(OpCodes.Stfld, typeBuilder.BaseType.GetField("_entityName", BindingFlags.Instance | BindingFlags.NonPublic));
            generator.Emit(OpCodes.Ret);
        }
    }
}