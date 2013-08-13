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
        private readonly IRepository<RelationshipColumnRecord> _relationshipColumnRepository;

        private readonly IRepository<ContentPartDefinitionRecord> _contentPartRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IRepository<RelationshipColumnRecord> relationshipColumn,
            IRepository<ContentPartDefinitionRecord> contentPartRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _relationshipColumnRepository = relationshipColumn;
            _contentPartRepository = contentPartRepository;
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

        public SelectListItem[] GetEntityNames() {
            var entities = _contentDefinitionManager.ListUserDefinedTypeDefinitions();
            return entities == null ? null :
                       (from entity in entities
                        select new SelectListItem {
                            Value = entity.Name,
                            Text = entity.DisplayName,
                            Selected = false
                        }).ToArray();
        }

        public RelationshipRecord[] GetRelationships(string entityName) {
            var entity = _contentPartRepository.Table.SingleOrDefault(part => part.Name == entityName);
            if (entity == null || entity.Id == 0) {
                return null;
            }
            return (from record in _relationshipRepository.Table
                    where record.PrimaryEntity.Id == entity.Id || record.RelatedEntity.Id == entity.Id
                    select record).ToArray();
        }

        ///<summary> 
        /// Lookup field not implemented
        ///</summary>
        public bool CreateRelationship(OneToManyRelationshipModel oneToMany) {
            if (oneToMany == null) {
                return false;
            }
            var primaryEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == oneToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == oneToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || primaryEntity.Id == 0 || relatedEntity.Id == 0
                || primaryEntity.Id == relatedEntity.Id) {
                return false;
            }
            if (GetExistedRelationId(oneToMany.Name, primaryEntity.Id, relatedEntity.Id) != -1) {
                return false;
            }

            var relationshipId = CreateRelation(new RelationshipRecord {
                Name = oneToMany.Name,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte) RelationshipType.OneToMany
            });

            _oneToManyRepository.Create(new OneToManyRelationshipRecord {
                DeleteOption = (byte)oneToMany.DeleteOption,
                LookupField = new ContentPartFieldDefinitionRecord(),
                RelatedListLabel = oneToMany.RelatedListLabel,
                Relationship = relationshipId,
                ShowRelatedList = oneToMany.ShowRelatedList
            });

            if (oneToMany.ColumnFieldList != null) {
                foreach (var colummn in oneToMany.ColumnFieldList) {
                    if (!CreateColumn(colummn, relatedEntity, relationshipId, true)) {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CreateRelationship(ManyToManyRelationshipModel manyToMany) {
            if (manyToMany == null) {
                return false;
            }
            var primaryEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == manyToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == manyToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || primaryEntity.Id == 0 || relatedEntity.Id == 0
                || primaryEntity.Id == relatedEntity.Id) {
                return false;
            }
            if (GetExistedRelationId(manyToMany.Name, primaryEntity.Id, relatedEntity.Id) != -1) {
                return false;
            }

            var relationship = CreateRelation(new RelationshipRecord {
                Name = manyToMany.Name,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte)RelationshipType.ManyToMany
            });

            _manyToManyRepository.Create(new ManyToManyRelationshipRecord {
                PrimaryListLabel = manyToMany.PrimaryListLabel,
                RelatedListLabel = manyToMany.RelatedListLabel,
                Relationship = relationship,
                ShowPrimaryList = manyToMany.ShowPrimaryList,
                ShowRelatedList = manyToMany.ShowRelatedList
            });

            if (manyToMany.PrimaryColumnList != null) {
                foreach (var colummn in manyToMany.PrimaryColumnList) {
                    if (!CreateColumn(colummn, primaryEntity, relationship, false)) {
                        return false;
                    }
                }
            }

            if (manyToMany.RelatedColumnList != null) {
                foreach (var colummn in manyToMany.RelatedColumnList) {
                    if (!CreateColumn(colummn, relatedEntity, relationship, true)) {
                        return false;
                    }
                }
            }

            return true;
        }

        private RelationshipRecord CreateRelation(RelationshipRecord relationship) {
            _relationshipRepository.Create(relationship);
            return relationship;
        }

        private bool CreateColumn(string fieldName, ContentPartDefinitionRecord entity, RelationshipRecord relationship, bool isRelated) {
            var field = entity.ContentPartFieldDefinitionRecords.First(f => f.Name == fieldName);
            if (field == null) {
                return false;
            }
            _relationshipColumnRepository.Create(new RelationshipColumnRecord {
                Column = field,
                IsRelatedList = isRelated,
                Relationship = relationship
            });
            return true;
        }

        private int GetExistedRelationId(string name, int primaryId, int relatedId) {
            var relation = _relationshipRepository.Table.SingleOrDefault(
                record => record.Name == name
                    && record.PrimaryEntity.Id == primaryId
                    && record.RelatedEntity.Id == relatedId);

            return relation != null && relation.Id != 0 ? relation.Id : -1;
        }
    }
}