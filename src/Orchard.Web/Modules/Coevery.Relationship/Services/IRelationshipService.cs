using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        string CreateRelationship(OneToManyRelationshipModel oneToMany);
        string CreateRelationship(ManyToManyRelationshipModel manyToMany);
        string DeleteRelationship(int relationshipId);
    }
}