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
        SelectListItem[] EntityNames { get; }
        bool CreateRelationship(OneToManyRelationshipModel oneToMany);
        SelectListItem[] GetFieldNames(string entityName);
    }
}