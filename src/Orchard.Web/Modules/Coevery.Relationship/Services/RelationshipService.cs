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
        private readonly ISessionLocator _sessionLocator;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IRepository<RelationshipColumnRecord> relationshipColumn,
            IRepository<ContentPartDefinitionRecord> contentPartRepository,
            ISessionLocator sessionLocator,
            IContentDefinitionManager contentDefinitionManager) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _relationshipColumnRepository = relationshipColumn;
            _contentPartRepository = contentPartRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _sessionLocator = sessionLocator;
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

        public SelectListItem[] GetEntityNames(string excludeEntity) {
            var entities = _contentDefinitionManager.ListUserDefinedTypeDefinitions();
            return entities == null ? null :
                       (from entity in entities
                        where entity.Name != excludeEntity
                        select new SelectListItem {
                            Value = entity.Name,
                            Text = entity.DisplayName,
                            Selected = false
                        }).ToArray();
        }

        public OneToManyRelationshipRecord GetOneToMany(int id) {
            return _oneToManyRepository.Fetch(record => record.Relationship.Id == id).FirstOrDefault();
        }

        public ManyToManyRelationshipRecord GetManyToMany(int id) {
            return _manyToManyRepository.Fetch(record=>record.Relationship.Id == id).FirstOrDefault();
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
        public string CreateRelationship(OneToManyRelationshipModel oneToMany) {
            if (oneToMany == null) {
                return "Invalid model.";
            }
            var primaryEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == oneToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == oneToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || primaryEntity.Id == 0 || relatedEntity.Id == 0
                || primaryEntity.Id == relatedEntity.Id) {
                return "Invalid entity";
            }
            if (GetExistedRelationId(oneToMany.Name) != -1) {
                return "Name already exist.";
            }

            var relationshipId = CreateRelation(new RelationshipRecord {
                Name = oneToMany.Name,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte) RelationshipType.OneToMany
            });

            var fieldStore = _sessionLocator.For(typeof (ContentPartFieldDefinitionRecord));
            var fieldTypeStore = _sessionLocator.For(typeof (ContentFieldDefinitionRecord));
            fieldStore.Save(new {});

            _oneToManyRepository.Create(new OneToManyRelationshipRecord {
                DeleteOption = (byte)oneToMany.DeleteOption,
                LookupField = new ContentPartFieldDefinitionRecord {
                    Id = 0
                },
                RelatedListLabel = oneToMany.RelatedListLabel,
                Relationship = relationshipId,
                ShowRelatedList = oneToMany.ShowRelatedList
            });

            if (oneToMany.ColumnFieldList != null) {
                foreach (var colummn in oneToMany.ColumnFieldList) {
                    if (!CreateColumn(colummn, relatedEntity, relationshipId, true)) {
                        return "Invalid field";
                    }
                }
            }
            return null;
        }

        public string CreateRelationship(ManyToManyRelationshipModel manyToMany) {
            if (manyToMany == null) {
                return "Invalid model.";
            }
            var primaryEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == manyToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == manyToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || primaryEntity.Id == 0 || relatedEntity.Id == 0
                || primaryEntity.Id == relatedEntity.Id) {
                return "Invalid entity";
            }
            if (GetExistedRelationId(manyToMany.Name) != -1) {
                return "Name already exist.";
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
                        return "Invalid field";
                    }
                }
            }

            if (manyToMany.RelatedColumnList != null) {
                foreach (var colummn in manyToMany.RelatedColumnList) {
                    if (!CreateColumn(colummn, relatedEntity, relationship, true)) {
                        return "Invalid field";
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Lookup field delete not completed
        /// </summary>
        /// <param name="relationshipId"></param>
        /// <returns>Error message string or null for correctly deleted</returns>
        public string DeleteRelationship(int relationshipId) {
            var relationship = _relationshipRepository.Get(relationshipId);
            if (relationship == null || relationship.Id != relationshipId) {
                return "Wrong relationship ID!";
            }

            DeleteColumns(relationshipId);

            if (relationship.Type == (byte) RelationshipType.ManyToMany) {
                var manyToMany = _manyToManyRepository.Get(record => record.Relationship.Id == relationshipId);
                if (manyToMany == null || manyToMany.Id == 0) {
                    return "Corresponding ManyToMany record not found.";
                }

                _manyToManyRepository.Delete(manyToMany);
            }
            else if (relationship.Type == (byte) RelationshipType.OneToMany) {
                var oneToMany = _oneToManyRepository.Get(record => record.Relationship.Id == relationshipId);
                if (oneToMany == null || oneToMany.Id == 0) {
                    return "Corresponding OneToMany record not found.";
                }

                var lookupField = (from entity in _contentPartRepository.Table
                                   from field in entity.ContentPartFieldDefinitionRecords
                                   where field.Id == oneToMany.LookupField.Id 
                                   //&& field.ContentFieldDefinitionRecord.Name == "ReferenceField"
                                   select field).First();
                if (oneToMany.DeleteOption == (byte) OneToManyDeleteOption.NotAllowed) {
                    if (lookupField != null && lookupField.Id != 0) {
                        return "Delete lookup field first.";
                    }
                    
                }
                else if (oneToMany.DeleteOption == (byte) OneToManyDeleteOption.CascadingDelete) {
                    if (lookupField != null && lookupField.Id != 0) {
                        var fieldRepository = _sessionLocator.For(typeof (ContentPartFieldDefinitionRecord));
                        fieldRepository.Delete(lookupField);
                    }     
                }
                _oneToManyRepository.Delete(oneToMany);
            }
            _relationshipRepository.Delete(relationship);

            return null;
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

        private int GetExistedRelationId(string name) {
            var relation = _relationshipRepository.Table.SingleOrDefault(
                record => record.Name == name);

            return relation != null && relation.Id != 0 ? relation.Id : -1;
        }

        private void DeleteColumns(int relationshipId) {
            var columnStore = _sessionLocator.For(typeof (RelationshipColumnRecord));
            var deleteCommand = "DELETE FROM dbo.Coevery_Relationship_RelationshipColumnRecord " +
                                   "WHERE Relationship_Id=" + relationshipId;
            var query = columnStore.CreateSQLQuery(deleteCommand);
            query.ExecuteUpdate();
        }
    }
}