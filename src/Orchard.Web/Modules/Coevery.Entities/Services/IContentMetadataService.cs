using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Models;
using Coevery.Entities.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;

namespace Coevery.Entities.Services {
    public interface IContentMetadataService : IDependency {
        bool CreateEntity(EditTypeViewModel sourceModel);
        IEnumerable<EntityMetadataPart> GetRawEntities();
        EditTypeViewModel GetEntity(int id);
    }

    public class ContentMetadataService : IContentMetadataService {
        private readonly IOrchardServices _services;
        private readonly ISettingsFormatter _settingsFormatter;
        private readonly IRepository<ContentFieldDefinitionRecord> _fieldDefinitionRepository;
        public ContentMetadataService(
            IOrchardServices services,
            ISettingsFormatter settingsFormatter,
            IRepository<ContentFieldDefinitionRecord> fieldDefinitionRepository
            ) {
            _services = services;
            _settingsFormatter = settingsFormatter;
            _fieldDefinitionRepository = fieldDefinitionRepository;
        }

        public IEnumerable<EntityMetadataPart> GetRawEntities() {
            return _services.ContentManager.Query<EntityMetadataPart, EntityMetadataRecord>()
                .ForVersion(VersionOptions.Latest).List();
        }

        public EditTypeViewModel GetEntity(int id) {
            var entity = _services.ContentManager.Get<EntityMetadataPart>(id,VersionOptions.Latest);
            if (entity == null) {
                return null;
            }
            return new EditTypeViewModel {
                DisplayName = entity.DisplayName,
                Name = entity.Name
            };
        }

        public bool CreateEntity(EditTypeViewModel sourceModel) {
            var entityDraft = _services.ContentManager.New<EntityMetadataPart>("EntityMetadata");
            var baseFieldSetting = new SettingsDictionary {
                { "DisplayName",sourceModel.FieldLabel },
                { "Storage","Part"},
                { "CoeveryTextFieldSettings.IsDispalyField",bool.TrueString},
                { "CoeveryTextFieldSettings.Required", bool.TrueString},
                { "CoeveryTextFieldSettings.ReadOnly", bool.TrueString},
                { "CoeveryTextFieldSettings.AlwaysInLayout", bool.TrueString},
                { "CoeveryTextFieldSettings.IsSystemField", bool.TrueString},
                { "CoeveryTextFieldSettings.IsAudit", bool.FalseString}
            };
            entityDraft.DisplayName = sourceModel.DisplayName;
            entityDraft.Name = sourceModel.Name;
            //entityDraft.Settings = sourceModel.Settings;
            var baseFieldDefinition = _fieldDefinitionRepository.Get(def => def.Name == "CoeveryTextField");
            if (baseFieldDefinition == null) {
                baseFieldDefinition = new ContentFieldDefinitionRecord { Name = "CoeveryTextField"};
                _fieldDefinitionRepository.Create(baseFieldDefinition);
            }

            entityDraft.FieldMetadataRecords.Add(new FieldMetadataRecord {
                //EntityMetadataRecord = 
                Name = sourceModel.FieldName,
                ContentFieldDefinitionRecord = baseFieldDefinition,
                Settings = _settingsFormatter.Map(baseFieldSetting).ToString()
            });
            _services.ContentManager.Create(entityDraft, VersionOptions.Draft);
            return true;
        }

        
    }
}

/*Abandoned code
 _contentDefinitionService.AlterType(viewModel, this);
            _contentDefinitionService.AddPartToType(viewModel.Name, viewModel.Name);
            _contentDefinitionService.AddPartToType("CoeveryCommonPart", viewModel.Name);

            _contentDefinitionManager.AlterPartDefinition(viewModel.Name,
                builder => builder.WithField(viewModel.FieldName,
                    fieldBuilder => fieldBuilder
                        .OfType("CoeveryTextField")
                        .WithDisplayName(viewModel.FieldLabel)
                        .WithSetting("CoeveryTextFieldSettings.IsDispalyField", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.Required", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.ReadOnly", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.AlwaysInLayout", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.IsSystemField", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.IsAudit", bool.FalseString)
                        .WithSetting("CoeveryTextFieldSettings.HelpText", string.Empty)
                        .WithSetting("Storage", "Part")));

            Services.Notifier.Information(T("The \"{0}\" content type has been created.", viewModel.DisplayName));
            _entityEvents.OnCreated(viewModel.Name);

            _schemaUpdateService.CreateTable(viewModel.Name,
                context => context.FieldColumn(viewModel.FieldName, "CoeveryTextField"));
 */