using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Fields.Records;
using Orchard;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;

namespace Coevery.Fields.Services {
    public class OptionItemService : IOptionItemService {
        private readonly IRepository<OptionItemRecord> _optionItemRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;

        public OptionItemService(IRepository<OptionItemRecord> optionItemRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository) {
            _optionItemRepository = optionItemRepository;
            _partDefinitionRepository = partDefinitionRepository;
        }

        private void ResetDefault(ContentPartFieldDefinitionRecord fieldDefinitionRecord) {
            var defaultItem = _optionItemRepository.Table.SingleOrDefault(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord && x.IsDefault);
            if (defaultItem != null) {
                defaultItem.IsDefault = false;
                _optionItemRepository.Update(defaultItem);
            }
        }

        public bool EditItem(int id, OptionItemRecord optionItem) {
            var optionItemRecord = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItemRecord == null) {
                return false;
            }
            if (optionItem.IsDefault) {
                ResetDefault(optionItemRecord.ContentPartFieldDefinitionRecord);
            }           
            optionItemRecord.Value = optionItem.Value;
            optionItemRecord.IsDefault = optionItem.IsDefault;
            _optionItemRepository.Update(optionItemRecord);
            return true;
        }

        public bool CreateItem(string entityName, string fieldName, OptionItemRecord optionItem) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return false;
            }
            var fieldDefinitionRecord = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (fieldDefinitionRecord == null) {
                return false;
            }
            optionItem.ContentPartFieldDefinitionRecord = fieldDefinitionRecord;
            if (optionItem.IsDefault) {
                ResetDefault(fieldDefinitionRecord);
            } 
            _optionItemRepository.Create(optionItem);
            return true;
        }

        public bool DeleteItem(int id) {
            var optionItem = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItem == null) {
                return false;
            }
            _optionItemRepository.Delete(optionItem);
            return true;
        }

        public object GetItemsForField(string entityName, string fieldName) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return null;
            }
            var fieldDefinitionRecord = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (fieldDefinitionRecord == null) {
                return null;
            }
            var items = _optionItemRepository.Table.Where(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord).Select(x => new { x.Id, x.IsDefault, x.Value });
            return items;
        }

        public List<SelectListItem> GetItemsForField(int fieldId) {

            return (from i in _optionItemRepository.Table
                    where i.ContentPartFieldDefinitionRecord.Id == fieldId
                    select new SelectListItem {
                        Value = i.Id.ToString(),
                        Text = i.Value,
                        Selected = i.IsDefault
                    }).ToList();
        }

        public void DeleteItemsForField(string fieldName, string entityName) {
            var field = _partDefinitionRepository.Table.Single(x => x.Name == entityName)
                                                 .ContentPartFieldDefinitionRecords.Single(f => f.Name == fieldName);
            if (field != null && field.ContentFieldDefinitionRecord.Name == "SelectField") {
                foreach (var item in (from option in _optionItemRepository.Table
                                      where option.ContentPartFieldDefinitionRecord == field
                                      select option)) {
                    _optionItemRepository.Delete(item);
                }
            }          
        }

        public int GetItemCountForField(int fieldId) {
            return (from i in _optionItemRepository.Table
                    where i.ContentPartFieldDefinitionRecord.Id == fieldId
                    select i).Count();
        }

        public int InitializeField(string entityName, string fieldName, string[] labels, int defaultValue) {
            var entity = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (entity == null) {
                return -1;
            }

            var field = entity.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (field == null || field.Id == 0) {
                return -1;
            }

            var idIndex = 0;
            foreach (var label in labels) {
                idIndex++;
                var option = new OptionItemRecord {
                    Value = label,
                    ContentPartFieldDefinitionRecord = field,
                    IsDefault = (idIndex == defaultValue)
                };
                _optionItemRepository.Create(option);
            }
            return field.Id;
        }
    }
}