using System.Web.Mvc;
using Coevery.Relationship.Models;
using Coevery.Relationship.Records;
using Orchard;

namespace Coevery.Relationship.Services {
    public interface IRelationshipService : IDependency {
        SelectListItem[] GetEntityNames(string excludeEntity);
        SelectListItem[] GetFieldNames(string entityName);
        RelationshipRecord[] GetRelationships(string entityName);
        OneToManyRelationshipRecord GetOneToMany(int id);
        ManyToManyRelationshipRecord GetManyToMany(int id);

        int CreateOneToManyRelationship(string fieldName, string relationName, string primaryEntityName, string relatedEntityName);
        string CreateRelationship(OneToManyRelationshipModel oneToMany);
        string CreateRelationship(ManyToManyRelationshipModel manyToMany);
        string EditRelationship(int relationshipId, ManyToManyRelationshipModel manyToMany);
        string EditRelationship(int relationshipId, OneToManyRelationshipModel oneToMany);
        void DeleteRelationship(RelationshipRecord relationship);
    }
}