using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using Coevery.Fields.Settings;
using Coevery.Fields.Records;
using Newtonsoft.Json.Converters;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;

namespace Coevery.Fields.Services {
    

    public class FieldDependencyService:IFieldDependencyService {
        private readonly IRepository<FieldDependencyRecord> _fieldDependencyRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public FieldDependencyService(
            IRepository<FieldDependencyRecord> fieldDependencyRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _fieldDependencyRepository = fieldDependencyRepository;
            _partDefinitionRepository = partDefinitionRepository;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public object Get(string entityName) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return null;
            }

            var dependencies = _fieldDependencyRepository.Table
                .Where(x => x.Entity == partDefinition)
                .Select(x => new {
                    x.Id,
                    ControlFieldName = x.ControlField.Name,
                    DependentFieldName = x.DependentField.Name
                });
            return dependencies;
        }

        public string GetDependencyMap(int dependentId) {           
            var dependencyMap = from entry in _fieldDependencyRepository.Table
                                    where entry.DependentField.Id == dependentId
                                    select entry.Value.RecoverValuePairs();
            if (!dependencyMap.Any()) {
                return null;
            }

            return JsonConvert.SerializeObject(
                dependencyMap.ToArray().SelectMany(dep => dep.AsEnumerable()),
                new KeyValuePairConverter());
        } 

        public bool Edit(int id,string newDependency) {
            var dependencyRecord = _fieldDependencyRepository.Table.SingleOrDefault(x => x.Id == id);
            if (newDependency == null || dependencyRecord == null) {
                return false;
            }
            dependencyRecord.Value = newDependency;
            _fieldDependencyRepository.Update(dependencyRecord);
            return true;
        }

        public bool Create(string entityName, string controlFieldName, string dependentFieldName, DependencyValuePair[] mappingValue) {
            if (mappingValue == null) {
                return false;
            }

            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return false;
            }
            var controlField = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == controlFieldName);
            var dependentField = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == dependentFieldName);
            if (controlField == null || dependentField == null) {
                return false;
            }
            
            var dependencyRecord = _fieldDependencyRepository.Table.SingleOrDefault(
                x => x.Entity == partDefinition && x.ControlField == controlField && x.DependentField == dependentField);
            if (dependencyRecord != null && dependencyRecord.Id != 0) {
                return false;
            }

            dependencyRecord = new FieldDependencyRecord {
                Entity = partDefinition,
                ControlField = controlField,
                DependentField = dependentField,
                Value = mappingValue.DependencyPairsToString(controlField.Name)
            };
            _fieldDependencyRepository.Create(dependencyRecord);
            return UpdateDependenySetting(entityName,controlFieldName,dependentFieldName);
        }

        public bool Delete(int id) {
            var optionItem = _fieldDependencyRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItem == null) {
                return false;
            }
            _fieldDependencyRepository.Delete(optionItem);
            return true;
        }

        private bool UpdateDependenySetting(string entityName, string controlFieldName, string dependentFieldName) {
            var part = _contentDefinitionManager.GetPartDefinition(entityName);
            if (part == null) {
                return false;
            }
            var control = part.Fields.FirstOrDefault(x => x.Name == controlFieldName);
            if (control == null) {
                return false;
            }
            control.Settings[control.FieldDefinition.Name+"Settings.DependencyMode"] = DependentType.Control.ToString();
            var dependent = part.Fields.FirstOrDefault(x => x.Name == dependentFieldName);
            if (dependent == null) {
                return false;
            }
            dependent.Settings["OptionSetFieldSettings.DependencyMode"] = DependentType.Dependent.ToString();
            _contentDefinitionManager.StorePartDefinition(part);
            return true;
        }
    }
}