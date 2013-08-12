using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Core.Models;
using Coevery.Relationship.Records;
using Coevery.Relationship.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;


namespace Coevery.Relationship.Services {
    public class RelationshipService : IRelationshipService {
        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRepository;
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRepository;
        private readonly IRepository<RelationshipColumnRecord> _relationshipColumn;

        private readonly IRepository<ContentTypeDefinitionRecord> _contentTypeRepository;
        private readonly IRepository<ViewPartRecord> _entityRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IRepository<RelationshipColumnRecord> relationshipColumn,
            IRepository<ViewPartRecord> entityRepository,            
            IRepository<ContentTypeDefinitionRecord> contentTypeRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _relationshipColumn = relationshipColumn;
            _contentTypeRepository = contentTypeRepository;
            _entityRepository = entityRepository;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public SelectListItem[] GetFieldNames(string entityName) {
            var entity = _contentDefinitionManager.GetPartDefinition(entityName);
            return entity == null ? null
                       : (from field in entity.Fields
                          select new SelectListItem {
                              Value = field.Name,
                              Text = field.DisplayName,
                              Selected = false
                          }).ToArray();
        }

        public SelectListItem[] EntityNames {
            get {
                return _entityRepository.Table.Join(_contentTypeRepository.Table
                                                    , entity => entity.ContentTypeDefinitionRecord_id
                                                    , contentType => contentType.Id
                                                    , (entity, contentType) => new SelectListItem {
                                                        Value = contentType.Name,
                                                        Text = contentType.DisplayName,
                                                        Selected = false
                                                    }).ToArray();
            }
        }

        public bool CreateRelationship(OneToManyRelationshipModel oneToMany) {
            if (oneToMany == null) {
                return false;
            }

            return true;
        }
    }
}