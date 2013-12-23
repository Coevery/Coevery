using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Coevery.Common.Extensions;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Data;
using Coevery.Entities.Services;
using Coevery.Relationship.Fields;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.DynamicTypeGeneration {
    public class ReferenceFieldGenerationProvider : FieldGenerationProvider<ReferenceField> {
        private readonly IRepository<OneToManyRelationshipRecord> _repository;

        public ReferenceFieldGenerationProvider(IRepository<OneToManyRelationshipRecord> repository) {
            _repository = repository;
        }

        protected override void GenerateProperty(TypeBuilder typeBuilder, ContentPartDefinition partDefinition, ContentPartFieldDefinition partFieldDefinition, Dictionary<string, TypeBuilder> typeBuilders) {
            string entityName = partDefinition.Name.RemovePartSuffix();
            string fieldName = partFieldDefinition.Name;

            var relationship = (from record in _repository.Table
                where record.Relationship.PrimaryEntity.ContentItemVersionRecord.Latest
                      && record.Relationship.RelatedEntity.ContentItemVersionRecord.Latest
                      && record.Relationship.RelatedEntity.Name == entityName
                      && record.LookupField.Name == fieldName
                select record).First();

            string primaryPartName = relationship.Relationship.PrimaryEntity.Name.ToPartName();
            var primaryTypeBuilder = typeBuilders[primaryPartName];

            DefineProperty(typeBuilder, partFieldDefinition.Name, primaryTypeBuilder);

            //DefineProperty(primaryTypeBuilder, "ListTest", typeof(IList<>).MakeGenericType(typeBuilder));
        }

        protected override string GenerateColumnName(string fieldName) {
            return fieldName + "_id";
        }
    }
}