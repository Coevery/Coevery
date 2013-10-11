using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Core.DynamicTypeGeneration;
using Coevery.Core.Services;
using Coevery.Entities.Services;
using Coevery.Entities.ViewModels;
using Coevery.Relationship.Records;
using Coevery.Relationship.Models;
using Coevery.Relationship.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Projections.Models;
using Orchard.Utility.Extensions;

namespace Coevery.Relationship.Services {
    public class RelationshipService : IRelationshipService {
        #region Class definition

        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRepository;
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRepository;

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentManager _contentManager;
        private readonly IContentMetadataService _contentMetadataService;

        public RelationshipService(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRepository,
            IContentDefinitionManager contentDefinitionManager,
            IDynamicAssemblyBuilder dynamicAssemblyBuilder,
            ISchemaUpdateService schemaUpdateService,
            IContentManager contentManager,
            IContentMetadataService contentMetadataService) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRepository = oneToManyRepository;
            _manyToManyRepository = manyToManyRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            _schemaUpdateService = schemaUpdateService;
            _contentManager = contentManager;
            _contentMetadataService = contentMetadataService;
        }

        #endregion

        #region GetMethods

        public string CheckRelationName(string name) {
            string errorMessage = null;
            if (!string.Equals(name, name.ToSafeName())) {
                errorMessage += "The name:\""+ name +"\" is not legal!\n";
            }
            if (_relationshipRepository.Fetch(relation => relation.Name == name).FirstOrDefault() != null) {
                errorMessage += "The name:\"" + name + "\" already exists!\n";
            }
            return errorMessage;
        }

        public string GetReferenceField(string entityName, string relationName) {
            var reference = _contentDefinitionManager
                .GetPartDefinition(entityName)
                .Fields.FirstOrDefault(field => field.FieldDefinition.Name == "ReferenceField"
                    && field.Settings.TryGetModel<ReferenceFieldSettings>().RelationshipName == relationName);
            return reference == null ? null : reference.Name;
        }

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
            var entity = _contentMetadataService.GetEntity(entityName);
            if (entity == null) {
                return null;
            }
            return (from record in _relationshipRepository.Table
                where record.PrimaryEntity.ContentItemVersionRecord.Latest
                    && record.RelatedEntity.ContentItemVersionRecord.Latest
                    && (record.PrimaryEntity == entity.Record || record.RelatedEntity == entity.Record)
                select record).ToArray();
        }

        #endregion

        #region CreateMethods

        public int CreateOneToManyRelationship(string fieldName, string relationName, string primaryEntityName, string relatedEntityName) {
            var primaryEntity = _contentMetadataService.GetEntity(primaryEntityName);
            var relatedEntity = _contentMetadataService.GetEntity(relatedEntityName);

            var fieldRecord = relatedEntity.FieldMetadataRecords.FirstOrDefault(field => field.Name == fieldName);
            if (fieldRecord == null) {
                return -1;
            }
            var relationship = CreateRelation(new RelationshipRecord {
                Name = relationName,
                PrimaryEntity = primaryEntity.Record,
                RelatedEntity = relatedEntity.Record,
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

        public string CreateRelationship(OneToManyRelationshipModel oneToMany) {
            if (oneToMany == null) {
                return "Invalid model.";
            }
            var primaryEntity = _contentMetadataService.GetEntity(oneToMany.PrimaryEntity);
            var relatedEntity = _contentMetadataService.GetDraftEntity(oneToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || !primaryEntity.HasPublished()) {
                return "Invalid entity";
            }
            if (RelationshipExists(oneToMany.Name)) {
                return "Name already exist.";
            }

            var relationship = CreateRelation(new RelationshipRecord {
                Name = oneToMany.Name,
                PrimaryEntity = primaryEntity.Record,
                RelatedEntity = relatedEntity.Record,
                Type = (byte) RelationshipType.OneToMany
            });

            var updateModel = new ReferenceUpdateModel(new ReferenceFieldSettings {
                AlwaysInLayout = oneToMany.AlwaysInLayout,
                ContentTypeName = oneToMany.PrimaryEntity,
                DisplayAsLink = oneToMany.DisplayAsLink,
                HelpText = oneToMany.HelpText,
                IsAudit = oneToMany.IsAudit,
                Required = oneToMany.Required,
                RelationshipId = relationship.Id,
                RelationshipName = relationship.Name
            });
            var fieldViewModel = new AddFieldViewModel {
                AddInLayout = true,
                Name = oneToMany.FieldName,
                DisplayName = oneToMany.FieldLabel,
                FieldTypeName = "ReferenceField"
            };
            _contentMetadataService.CreateField(relatedEntity, fieldViewModel, updateModel);

            var fieldRecord = relatedEntity.FieldMetadataRecords.FirstOrDefault(field => field.Name == oneToMany.FieldName);
            var projectionPart = CreateProjection(oneToMany.RelatedEntity, oneToMany.ColumnFieldList);

            _oneToManyRepository.Create(new OneToManyRelationshipRecord {
                DeleteOption = (byte) oneToMany.DeleteOption,
                LookupField = fieldRecord,
                RelatedListProjection = projectionPart.Record,
                RelatedListLabel = oneToMany.RelatedListLabel,
                Relationship = relationship,
                ShowRelatedList = oneToMany.ShowRelatedList
            });

            return relationship.Id.ToString();
        }

        public string CreateRelationship(ManyToManyRelationshipModel manyToMany) {
            if (manyToMany == null) {
                return "Invalid model.";
            }
            var primaryEntity = _contentMetadataService.GetEntity(manyToMany.PrimaryEntity);
            var relatedEntity = _contentMetadataService.GetEntity(manyToMany.RelatedEntity);
            if (primaryEntity == null || relatedEntity == null
                || !primaryEntity.HasPublished() || !relatedEntity.HasPublished()) {
                return "Invalid entity";
            }
            if (RelationshipExists(manyToMany.Name)) {
                return "Name already exist.";
            }

            var relationship = CreateRelation(new RelationshipRecord {
                Name = manyToMany.Name,
                PrimaryEntity = primaryEntity.Record,
                RelatedEntity = relatedEntity.Record,
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
            return relationship.Id.ToString();
        }

        #endregion

        #region Delete And Edit

        public void DeleteRelationship(RelationshipRecord relationship) {
            if (relationship == null) {
                return;
            }

            if (relationship.Type == (byte) RelationshipType.OneToMany) {
                var entity = _contentMetadataService.GetDraftEntity(relationship.RelatedEntity.Name);
                var record = _oneToManyRepository.Get(x => x.Relationship.Name == relationship.Name
                    && x.Relationship.RelatedEntity.ContentItemVersionRecord.Latest);
                var field = entity.FieldMetadataRecords.First(x => x.Name == record.LookupField.Name);
                entity.FieldMetadataRecords.Remove(field);
                DeleteRelationship(record);
            }
            else if (relationship.Type == (byte) RelationshipType.ManyToMany) {
                var record = _manyToManyRepository.Get(x => x.Relationship == relationship);
                DeleteRelationship(record);
            }
        }

        public void DeleteRelationship(OneToManyRelationshipRecord record) {
            if (record == null) {
                return;
            }
            record.RelatedListProjection.QueryPartRecord.Layouts.Clear();
            record.RelatedListProjection.QueryPartRecord.FilterGroups.Clear();
            _oneToManyRepository.Delete(record);
            _relationshipRepository.Delete(record.Relationship);
        }

        private void DeleteRelationship(ManyToManyRelationshipRecord record) {
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
            _schemaUpdateService.DropCustomTable(string.Format("Coevery_DynamicTypes_{0}ContentLinkRecord", record.Relationship.Name));
            _schemaUpdateService.DropCustomTable(string.Format("Coevery_DynamicTypes_{0}{1}PartRecord", record.Relationship.Name, primaryName));
            _schemaUpdateService.DropCustomTable(string.Format("Coevery_DynamicTypes_{0}{1}PartRecord", record.Relationship.Name, relatedName));

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

            UpdateLayoutProperties(relationshipRecord.PrimaryEntity.Name,
                manyToManyRecord.PrimaryListProjection.LayoutRecord,
                manyToMany.PrimaryColumnList);

            UpdateLayoutProperties(relationshipRecord.RelatedEntity.Name,
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

            UpdateLayoutProperties(oneToManyRecord.Relationship.RelatedEntity.Name,
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
            string category = typeName + "ContentFields";
            const string settingName = "CoeveryTextFieldSettings.IsDispalyField";
            var allFields = _contentDefinitionManager.GetPartDefinition(typeName).Fields.ToList();
            foreach (var property in properties) {
                var field = allFields.FirstOrDefault(c => c.Name == property);
                if (field == null) {
                    continue;
                }
                var propertyRecord = new PropertyRecord {
                    Category = category,
                    Type = string.Format("{0}.{1}.", typeName, property),
                    Description = field.DisplayName,
                    Position = layoutRecord.Properties.Count,
                    LinkToContent = field.Settings.ContainsKey(settingName) && bool.Parse(field.Settings[settingName])
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

            _schemaUpdateService.CreateCustomTable(
                "Coevery_DynamicTypes_" + primaryName + "PartRecord",
                table => table.ContentPartRecord()
                );
            _schemaUpdateService.CreateCustomTable(
                "Coevery_DynamicTypes_" + relatedName + "PartRecord",
                table => table.ContentPartRecord()
                );
            _schemaUpdateService.CreateCustomTable(
                "Coevery_DynamicTypes_" + manyToMany.Name + "ContentLinkRecord",
                table => table.Column<int>("Id", column => column.PrimaryKey().Identity())
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
            return _relationshipRepository.Table
                .Where(x => x.PrimaryEntity.ContentItemVersionRecord.Latest
                            && x.RelatedEntity.ContentItemVersionRecord.Latest)
                .Any(record => record.Name == name);
        }

        #endregion
    }
}