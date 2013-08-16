using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Core.Controllers;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Coevery.Entities.Services;
using Coevery.Entities.ViewModels;
using Coevery.Relationship.Controllers;
using Coevery.Relationship.Records;
using Coevery.Relationship.Models;
using Coevery.Relationship.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Settings.Controllers;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;


namespace Coevery.Relationship.Services {
    public class RelationshipService : IRelationshipService {
        #region Class definition
        private readonly string _tableFormat = "Coevery_Dynamic_ManyToManyRelationship_{0}Record";

        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRepository;
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRepository;
        private readonly IRepository<RelationshipColumnRecord> _relationshipColumnRepository;        

        private readonly IRepository<ContentPartDefinitionRecord> _contentPartRepository;
        private readonly ISessionLocator _sessionLocator;
        private readonly IFieldService _fieldService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionService _contentDefinitionService;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IRepository<RelationshipColumnRecord> relationshipColumn,
            IRepository<ContentPartDefinitionRecord> contentPartRepository,
            ISessionLocator sessionLocator,
            IFieldService fieldService,
            IContentDefinitionManager contentDefinitionManager, 
            IContentDefinitionService contentDefinitionService) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _relationshipColumnRepository = relationshipColumn;
            _contentPartRepository = contentPartRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionService = contentDefinitionService;
            _fieldService = fieldService;
            _schemaUpdateService = schemaUpdateService;
            _sessionLocator = sessionLocator;
        }

        #endregion

        #region GetMethods
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
        #endregion

        #region CreateMethods

        public int CreateOneToManyRelationship(string fieldName, string relationName, string primaryEntityName, string relatedEntityName) {
            var primaryEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == primaryEntityName);
            var relatedEntity = _contentPartRepository.Table.SingleOrDefault(entity => entity.Name == relatedEntityName);

            var fieldRecord = relatedEntity.ContentPartFieldDefinitionRecords.SingleOrDefault(field=>field.Name == fieldName);
            if (fieldRecord == null || fieldRecord.Id == 0) {
                return -1;
            }
            var relationship = CreateRelation(new RelationshipRecord {
                Name = relationName,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte)RelationshipType.OneToMany
            });

            var oneToMany = new OneToManyRelationshipRecord {
                DeleteOption = (byte)OneToManyDeleteOption.CascadingDelete,
                LookupField = fieldRecord,
                RelatedListLabel = relatedEntityName,
                Relationship = relationship,
                ShowRelatedList = false
            };
            _oneToManyRepository.Create(oneToMany);
            return oneToMany.Id;
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

            var relationship = CreateRelation(new RelationshipRecord {
                Name = oneToMany.Name,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte) RelationshipType.OneToMany
            });

            var updateModel = new ReferenceUpdateModel(new ReferenceFieldSettings {
                AlwaysInLayout = oneToMany.AlwaysInLayout,
                ContentTypeName = oneToMany.PrimaryEntity,
                DisplayAsLink = oneToMany.DisplayAsLink,
                HelpText = oneToMany.HelpText,
                IsAudit = oneToMany.IsAudit,
                IsSystemField = oneToMany.IsSystemField,
                ReadOnly = oneToMany.ReadOnly,
                Required = oneToMany.Required,
                RelationshipId = relationship.Id,
                RelationshipName = relationship.Name
            });
            _contentDefinitionService.AddFieldToPart(oneToMany.FieldName,oneToMany.FieldLabel, "ReferenceField", relatedEntity.Name);
            _contentDefinitionService.AlterField(relatedEntity.Name, new EditPartFieldViewModel {Name = oneToMany.FieldName}, updateModel);
            var fieldRecord = relatedEntity.ContentPartFieldDefinitionRecords.SingleOrDefault(field => field.Name == oneToMany.FieldName);

            _oneToManyRepository.Create(new OneToManyRelationshipRecord {
                DeleteOption = (byte) oneToMany.DeleteOption,
                LookupField = fieldRecord,
                RelatedListLabel = oneToMany.RelatedListLabel,
                Relationship = relationship,
                ShowRelatedList = oneToMany.ShowRelatedList
            });

            if (oneToMany.ColumnFieldList != null) {
                foreach (var colummn in oneToMany.ColumnFieldList) {
                    if (!CreateColumn(colummn, relatedEntity, relationship, true)) {
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

            _schemaUpdateService.CreateTable(_tableFormat, relationship.Name);

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
        #endregion

        #region Delete And Edit
        /// <summary>
        /// Delete OneToMany and ManyToMany Relationship
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
                _schemaUpdateService.DropTable(_tableFormat, relationship.Name);
            }
            else if (relationship.Type == (byte) RelationshipType.OneToMany) {
                var oneToMany = _oneToManyRepository.Get(record => record.Relationship.Id == relationshipId);
                if (oneToMany == null || oneToMany.Id == 0) {
                    return "Corresponding OneToMany record not found.";
                }
              
                var lookupFieldSet = (from entity in _contentPartRepository.Table
                                   from field in entity.ContentPartFieldDefinitionRecords
                                   where field.Id == oneToMany.LookupField.Id 
                                   //&& field.ContentFieldDefinitionRecord.Name == "ReferenceField"
                                   select field);
                var lookupField = lookupFieldSet.Any() ? lookupFieldSet.First() : null;

                if (oneToMany.DeleteOption == (byte) OneToManyDeleteOption.NotAllowed) {
                    if (lookupField != null && lookupField.Id != 0) {
                        return "Delete lookup field first.";
                    }                   
                }
                else if (oneToMany.DeleteOption == (byte) OneToManyDeleteOption.CascadingDelete) {
                    if (lookupField != null && lookupField.Id != 0) {
                        _fieldService.Delete(oneToMany.LookupField.Name, relationship.RelatedEntity.Name);
                    }     
                }
                _oneToManyRepository.Delete(oneToMany);
            }
            _relationshipRepository.Delete(relationship);
            return null;
        }

        /// <summary>
        /// Column alter method is not the best.
        /// </summary>
        /// <param name="relationshipId"></param>
        /// <param name="manyToMany"></param>
        /// <returns>Error message string or null for correctly edited</returns>
        public string EditRelationship(int relationshipId, ManyToManyRelationshipModel manyToMany) {
            var manyToManyRecord = _manyToManyRepository.Get(record => record.Relationship.Id == relationshipId);
            if (manyToManyRecord == null || manyToManyRecord.Id == 0) {
                return "Invalid relashionship ID.";
            }
            manyToManyRecord.ShowPrimaryList = manyToMany.ShowPrimaryList;
            manyToManyRecord.PrimaryListLabel = manyToMany.PrimaryListLabel;
            manyToManyRecord.ShowRelatedList = manyToMany.ShowRelatedList;
            manyToManyRecord.RelatedListLabel = manyToMany.RelatedListLabel;

            DeleteColumns(relationshipId);
            _manyToManyRepository.Update(manyToManyRecord);

            if (manyToMany.PrimaryColumnList != null) {
                foreach (var colummn in manyToMany.PrimaryColumnList) {
                    if (!CreateColumn(colummn, manyToManyRecord.Relationship.PrimaryEntity, manyToManyRecord.Relationship, false)) {
                        return "Invalid field";
                    }
                }
            }

            if (manyToMany.RelatedColumnList != null) {
                foreach (var colummn in manyToMany.RelatedColumnList) {
                    if (!CreateColumn(colummn, manyToManyRecord.Relationship.RelatedEntity, manyToManyRecord.Relationship, true)) {
                        return "Invalid field";
                    }
                }
            }
            return null;
        }

        public string EditRelationship(int relationshipId, OneToManyRelationshipModel oneToMany) {
            var oneToManyRecord = _oneToManyRepository.Get(record => record.Relationship.Id == relationshipId);
            if (oneToManyRecord == null || oneToManyRecord.Id == 0) {
                return "Invalid relashionship ID.";
            }

            oneToManyRecord.ShowRelatedList = oneToMany.ShowRelatedList;
            oneToManyRecord.RelatedListLabel = oneToMany.RelatedListLabel;

            DeleteColumns(relationshipId);
            _oneToManyRepository.Update(oneToManyRecord);

            if (oneToMany.ColumnFieldList != null) {
                foreach (var colummn in oneToMany.ColumnFieldList) {
                    if (!CreateColumn(colummn, oneToManyRecord.Relationship.RelatedEntity, oneToManyRecord.Relationship, true)) {
                        return "Invalid field";
                    }
                }
            }
            return null;
        }
        #endregion

        #region PrivateMethods
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

        //private bool AlterColumns(int relationshipId, string[] primaryList, string[] relatedList) {
        //    var columnStore = _sessionLocator.For(typeof(RelationshipColumnRecord));
        //    var fieldStore = _sessionLocator.For(typeof(ContentPartFieldDefinitionRecord));
        //    return true;
        //}

        //private bool AlterColumns(int relationshipId, string[] columnList, bool isRelated) {
        //    var columnStore = _sessionLocator.For(typeof (RelationshipColumnRecord));
        //    var fieldStore = _sessionLocator.For(typeof (ContentPartFieldDefinitionRecord));
        //    var createStringArray = "CREATE TYPE string_list AS TABLE (name nvarchar(40) PRIMARY KEY) "; 
        //    return true;
        //}

        private void DeleteColumns(int relationshipId) {
            var columnStore = _sessionLocator.For(typeof (RelationshipColumnRecord));
            var deleteCommand = "DELETE FROM dbo.Coevery_Relationship_RelationshipColumnRecord " +
                                   "WHERE Relationship_Id=" + relationshipId;
            //var transaction = columnStore.BeginTransaction();
            var query = columnStore.CreateSQLQuery(deleteCommand);
            query.ExecuteUpdate();
            //transaction.Commit();
        }
        #endregion
    }
}