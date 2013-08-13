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
        SelectListItem[] GetEntityNames();
        SelectListItem[] GetFieldNames(string entityName);
        RelationshipRecord[] GetRelationships(string entityName);
        bool CreateRelationship(OneToManyRelationshipModel oneToMany);
        bool CreateRelationship(ManyToManyRelationshipModel manyToMany);       
    }
}