using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Core.DynamicTypeGeneration;
using Coevery.Core.Services;
using Coevery.Entities.Services;
using Coevery.Relationship.Records;
using Coevery.Relationship.Models;
using Coevery.Relationship.Settings;
using NHibernate.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Migration.Schema;
using Orchard.FileSystems.VirtualPath;
using Orchard.Projections.Models;
using Orchard.Projections.Services;

namespace Coevery.Relationship.Services {
    public class RelationshipService : IRelationshipService {
        #region Class definition

        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRepository;
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRepository;

        private readonly IRepository<ContentPartDefinitionRecord> _contentPartRepository;
        private readonly ISessionLocator _sessionLocator;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly SchemaBuilder _schemaBuilder;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IRepository<ContentPartDefinitionRecord> contentPartRepository,
            ISessionLocator sessionLocator,
            IContentDefinitionManager contentDefinitionManager,
            IContentDefinitionService contentDefinitionService,
            IVirtualPathProvider virtualPathProvider,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            IDataMigrationInterpreter interpreter,
            ISchemaUpdateService schemaUpdateService,
            IContentManager contentManager,
            IProjectionManager projectionManager) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _contentPartRepository = contentPartRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionService = contentDefinitionService;
            _virtualPathProvider = virtualPathProvider;
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _interpreter = interpreter;
            _schemaUpdateService = schemaUpdateService;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
            _sessionLocator = sessionLocator;
            _schemaBuilder = new SchemaBuilder(_interpreter, "", s => s.Replace(".", "_"));
        }

        #endregion

        #region GetMethods

        public SelectListItem[] GetFieldNames(string entityName) {
            var entity = _contentDefinitionManager.GetPartDefinition(entityName);
            return entity == null
                ? null
                : (from field in entity.Fields
                    select new SelectListItem {
                        Value = field.Name,
                        Text = field.DisplayName,
                        Selected = false
                    }).ToArray();
        }

        public SelectListItem[] GetEntityNames(string excludeEntity) {
            var entities = _contentDefinitionManager.ListUserDefinedTypeDefinitions();
            return entities == null
                ? null
                : (from entity in entities
                    where entity.Name != excludeEntity
                    select new SelectListItem {
                        Value = entity.Name,
                        Text = entity.DisplayName,
                        Selected = false
                    }).ToArray();
        }

        public OneToManyRelationshipRecord GetOneToMany(int id) {
            return _oneToManyRepository.Get(record => record.Relationship.Id == id);
        }

        public ManyToManyRelationshipRecord GetManyToMany(int id) {
            return _manyToManyRepository.Get(record => record.Relationship.Id == id);
        }

        public RelationshipRecord[] GetRelationships(string entityName) {
            var entity = _contentPartRepository.Table.FirstOrDefault(part => part.Name == entityName);
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
            var primaryEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == primaryEntityName);
            var relatedEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == relatedEntityName);

            var fieldRecord = relatedEntity.ContentPartFieldDefinitionRecords.FirstOrDefault(field => field.Name == fieldName);
            if (fieldRecord == null || fieldRecord.Id == 0) {
                return -1;
            }
            var relationship = CreateRelation(new RelationshipRecord {
                Name = relationName,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte) RelationshipType.OneToMany
            });

            var projectionPart = CreateProjection(relatedEntityName, null);
            
            var oneToMany = new OneToManyRelationshipRecord {
                DeleteOption = (byte) OneToManyDeleteOption.CascadingDelete,
                LookupField = fieldRecord,
                RelatedListProjection = projectionPart.Record,
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
            var primaryEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == oneToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == oneToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || primaryEntity.Id == relatedEntity.Id) {
                return "Invalid entity";
            }
            if (RelationshipExists(oneToMany.Name)) {
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
                IsSystemField = false,
                ReadOnly = false,
                Required = oneToMany.Required,
                RelationshipId = relationship.Id,
                RelationshipName = relationship.Name
            });
            _contentDefinitionService.AddFieldToPart(oneToMany.FieldName, oneToMany.FieldLabel, "ReferenceField", relatedEntity.Name);
            _contentDefinitionService.AlterField(relatedEntity.Name, oneToMany.FieldName, updateModel);

            var fieldRecord = relatedEntity.ContentPartFieldDefinitionRecords.FirstOrDefault(field => field.Name == oneToMany.FieldName);
            var projectionPart = CreateProjection(oneToMany.RelatedEntity, oneToMany.ColumnFieldList);

            _oneToManyRepository.Create(new OneToManyRelationshipRecord {
                DeleteOption = (byte) oneToMany.DeleteOption,
                LookupField = fieldRecord,
                RelatedListProjection = projectionPart.Record,
                RelatedListLabel = oneToMany.RelatedListLabel,
                Relationship = relationship,
                ShowRelatedList = oneToMany.ShowRelatedList
            });

            _schemaUpdateService.CreateColumn(relatedEntity.Name, oneToMany.FieldName, "ReferenceField");
            return null;
        }

        public string CreateRelationship(ManyToManyRelationshipModel manyToMany) {
            if (manyToMany == null) {
                return "Invalid model.";
            }
            var primaryEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == manyToMany.PrimaryEntity);
            var relatedEntity = _contentPartRepository.Table.FirstOrDefault(entity => entity.Name == manyToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null || primaryEntity.Id == relatedEntity.Id) {
                return "Invalid entity";
            }
            if (RelationshipExists(manyToMany.Name)) {
                return "Name already exist.";
            }

            var relationship = CreateRelation(new RelationshipRecord {
                Name = manyToMany.Name,
                PrimaryEntity = primaryEntity,
                RelatedEntity = relatedEntity,
                Type = (byte) RelationshipType.ManyToMany
            });

            var primaryProjectionPart = CreateProjection(manyToMany.PrimaryEntity, manyToMany.PrimaryColumnList);
            var relatedProjectionPart = CreateProjection(manyToMany.RelatedEntity, manyToMany.RelatedColumnList);

            _manyToManyRepository.Create(new ManyToManyRelationshipRecord {
                PrimaryListProjection = primaryProjectionPart.Record,
                PrimaryListLabel = manyToMany.PrimaryListLabel,
                RelatedListProjection = relatedProjectionPart.Record,
                RelatedListLabel = manyToMany.RelatedListLabel,
                Relationship = relationship,
                ShowPrimaryList = manyToMany.ShowPrimaryList,
                ShowRelatedList = manyToMany.ShowRelatedList
            });

            GenerateManyToManyParts(manyToMany);
            return null;
        }

        #endregion

        #region Delete And Edit

        public void DeleteOneToManyRelationship(OneToManyRelationshipRecord record) {
            if (record == null) {
                return;
            }
            record.RelatedListProjection.QueryPartRecord.Layouts.Clear();
            record.RelatedListProjection.QueryPartRecord.FilterGroups.Clear();
            _oneToManyRepository.Delete(record);
            _relationshipRepository.Delete(record.Relationship);
        }

        public void DeleteManyToManyRelationship(ManyToManyRelationshipRecord record) {
            if (record == null) {
                return;
            }
            record.PrimaryListProjection.QueryPartRecord.Layouts.Clear();
            record.PrimaryListProjection.QueryPartRecord.FilterGroups.Clear();
            record.RelatedListProjection.QueryPartRecord.Layouts.Clear();
            record.RelatedListProjection.QueryPartRecord.FilterGroups.Clear();
            _manyToManyRepository.Delete(record);
            _relationshipRepository.Delete(record.Relationship);

            string primaryName = record.Relationship.PrimaryEntity.Name;
            string relatedName = record.Relationship.RelatedEntity.Name;
            _schemaBuilder.DropTable(string.Format("Coevery_DynamicTypes_{0}ContentLinkRecord", record.Relationship.Name));
            _schemaBuilder.DropTable(string.Format("Coevery_DynamicTypes_{0}{1}PartRecord", record.Relationship.Name, primaryName));
            _schemaBuilder.DropTable(string.Format("Coevery_DynamicTypes_{0}{1}PartRecord", record.Relationship.Name, relatedName));
            _contentDefinitionManager.DeletePartDefinition(record.Relationship.Name + primaryName + "Part");
            _contentDefinitionManager.DeletePartDefinition(record.Relationship.Name + relatedName + "Part");
            _dynamicAssemblyBuilder.Build();
        }

        /// <summary>
        /// Column alter method is not the best.
        /// </summary>
        /// <param name="relationshipId"></param>
        /// <param name="manyToMany"></param>
        /// <returns>Error message string or null for correctly edited</returns>
        public string EditRelationship(int relationshipId, ManyToManyRelationshipModel manyToMany) {
            var manyToManyRecord = _manyToManyRepository.Get(record => record.Relationship.Id == relationshipId);
            if (manyToManyRecord == null) {
                return "Invalid relashionship ID.";
            }
            var relationshipRecord = manyToManyRecord.Relationship;
            manyToManyRecord.ShowPrimaryList = manyToMany.ShowPrimaryList;
            manyToManyRecord.PrimaryListLabel = manyToMany.PrimaryListLabel;
            manyToManyRecord.ShowRelatedList = manyToMany.ShowRelatedList;
            manyToManyRecord.RelatedListLabel = manyToMany.RelatedListLabel;

            var primaryPart = _contentDefinitionManager.GetPartDefinition(relationshipRecord.Name + relationshipRecord.PrimaryEntity.Name + "Part");
            primaryPart.Settings["DisplayName"] = manyToMany.RelatedListLabel;
            _contentDefinitionManager.StorePartDefinition(primaryPart);
            var relatedPart = _contentDefinitionManager.GetPartDefinition(relationshipRecord.Name + relationshipRecord.RelatedEntity.Name + "Part");
            relatedPart.Settings["DisplayName"] = manyToMany.PrimaryListLabel;
            _contentDefinitionManager.StorePartDefinition(relatedPart);

            _manyToManyRepository.Update(manyToManyRecord);

            UpdateLayoutProperties(
                relationshipRecord.PrimaryEntity.Name,
                manyToManyRecord.PrimaryListProjection.LayoutRecord,
                manyToMany.PrimaryColumnList);
            UpdateLayoutProperties(
                relationshipRecord.RelatedEntity.Name,
                manyToManyRecord.RelatedListProjection.LayoutRecord,
                manyToMany.RelatedColumnList);
            return null;
        }

        public string EditRelationship(int relationshipId, OneToManyRelationshipModel oneToMany) {
            var oneToManyRecord = _oneToManyRepository.Get(record => record.Relationship.Id == relationshipId);
            if (oneToManyRecord == null) {
                return "Invalid relashionship ID.";
            }

            oneToManyRecord.ShowRelatedList = oneToMany.ShowRelatedList;
            oneToManyRecord.RelatedListLabel = oneToMany.RelatedListLabel;
            _oneToManyRepository.Update(oneToManyRecord);

            UpdateLayoutProperties(
                oneToManyRecord.Relationship.RelatedEntity.Name,
                oneToManyRecord.RelatedListProjection.LayoutRecord,
                oneToMany.ColumnFieldList);
            return null;
        }

        #endregion

        #region PrivateMethods

        private void UpdateLayoutProperties(string typeName, LayoutRecord layoutRecord, IEnumerable<string> properties) {
            layoutRecord.Properties.Clear();
            if (properties == null) {
                return;
            }
            var allFields = _projectionManager.DescribeProperties().SelectMany(x => x.Descriptors).ToList();
            string category = typeName + "ContentFields";
            foreach (var property in properties) {
                var field = allFields.FirstOrDefault(c => c.Category == category && c.Type.StartsWith(typeName + "." + property));
                if (field == null) {
                    continue;
                }
                var propertyRecord = new PropertyRecord {
                    Category = category,
                    Type = field.Type,
                    Description = field.Name.Text.Split(':').Length > 0 ? field.Name.Text.Split(':')[0] : string.Empty,
                    Position = layoutRecord.Properties.Count,
                    LinkToContent = layoutRecord.Properties.Count == 0
                };
                layoutRecord.Properties.Add(propertyRecord);
            }
        }

        private ProjectionPart CreateProjection(string typeName, IEnumerable<string> properties) {
            var projectionItem = _contentManager.New("ProjectionPage");
            var queryItem = _contentManager.New("Query");
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPart = queryItem.As<QueryPart>();
            _contentManager.Create(queryItem);

            projectionPart.Record.QueryPartRecord = queryPart.Record;
            _contentManager.Create(projectionItem);

            var layoutRecord = new LayoutRecord {Category = "Html", Type = "ngGrid"};
            queryPart.Record.Layouts.Add(layoutRecord);
            projectionPart.Record.LayoutRecord = layoutRecord;

            var filterGroup = new FilterGroupRecord();
            queryPart.Record.FilterGroups.Clear();
            queryPart.Record.FilterGroups.Add(filterGroup);
            var filterRecord = new FilterRecord {
                Category = "Content",
                Type = "ContentTypes",
                Position = filterGroup.Filters.Count,
                State = string.Format("<Form><ContentTypes>{0}</ContentTypes></Form>", typeName)
            };
            filterGroup.Filters.Add(filterRecord);

            UpdateLayoutProperties(typeName, layoutRecord, properties);
            return projectionPart;
        }

        private void GenerateManyToManyParts(ManyToManyRelationshipModel manyToMany) {
            var primaryName = manyToMany.Name + manyToMany.PrimaryEntity;
            var relatedName = manyToMany.Name + manyToMany.RelatedEntity;

            _schemaBuilder.CreateTable(
                "Coevery_DynamicTypes_" + primaryName + "PartRecord",
                table => table.ContentPartRecord()
                );
            _schemaBuilder.CreateTable(
                "Coevery_DynamicTypes_" + relatedName + "PartRecord",
                table => table.ContentPartRecord()
                );
            _schemaBuilder.CreateTable(
                "Coevery_DynamicTypes_" + manyToMany.Name + "ContentLinkRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("PrimaryPartRecord_Id")
                    .Column<int>("RelatedPartRecord_Id")
                );

            _contentDefinitionManager.AlterPartDefinition(
                primaryName + "Part",
                builder => builder
                    .Attachable()
                    .WithSetting("DisplayName", manyToMany.RelatedListLabel));

            _contentDefinitionManager.AlterPartDefinition(
                relatedName + "Part",
                builder => builder
                    .Attachable()
                    .WithSetting("DisplayName", manyToMany.PrimaryListLabel));

            _contentDefinitionManager.AlterTypeDefinition(manyToMany.PrimaryEntity, typeBuilder => typeBuilder.WithPart(primaryName + "Part"));
            _contentDefinitionManager.AlterTypeDefinition(manyToMany.RelatedEntity, typeBuilder => typeBuilder.WithPart(relatedName + "Part"));
            _dynamicAssemblyBuilder.Build();
        }

        private RelationshipRecord CreateRelation(RelationshipRecord relationship) {
            _relationshipRepository.Create(relationship);
            return relationship;
        }

        private bool RelationshipExists(string name) {
            var relation = _relationshipRepository.Table.FirstOrDefault(record => record.Name == name);
            return relation != null;
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
            //var columnStore = _sessionLocator.For(typeof (RelationshipColumnRecord));
            //var deleteCommand = "DELETE FROM dbo.Coevery_Relationship_RelationshipColumnRecord " +
            //                    "WHERE Relationship_Id=" + relationshipId;
            //var transaction = columnStore.BeginTransaction();
            //var query = columnStore.CreateSQLQuery(deleteCommand);
            //query.ExecuteUpdate();
            //transaction.Commit();
        }

        #endregion
    }
}