using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Fields.Records;
using Coevery.Fields.Settings;
using Orchard;
using Orchard.Logging;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;

namespace Coevery.Fields.Services {
    public class OptionItemService : IOptionItemService {
        private readonly IRepository<OptionSetRecord> _optionSetRepository;
        private readonly IRepository<OptionItemRecord> _optionItemRepository; 
        private readonly IRepository<SelectedOptionSetRecord> _selectedOptionSetRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public OptionItemService(IRepository<OptionSetRecord> optionSetRepository,
            IRepository<SelectedOptionSetRecord> selectedOptionSetRepository,
            IRepository<OptionItemRecord> optionItemRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _optionSetRepository = optionSetRepository;
            _selectedOptionSetRepository = selectedOptionSetRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _optionItemRepository = optionItemRepository;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        private OptionItemRecord[] GetItemsFromSet(int setId) {
            return _optionItemRepository.Table.Where(item => item.OptionSetRecord.Id == setId).ToArray();
        }

        private int GetOptionSetId(string entityName, string fieldName) {
            var field = _contentDefinitionManager.GetPartDefinition(entityName).Fields.SingleOrDefault(f => f.Name == fieldName);

            if (field != null && (field.FieldDefinition.Name == "OptionSetField")) {
                return field.Settings.GetModel<OptionSetFieldSettings>().OptionSetId;
            }
            return -1;
        }

        private void ResetDefault(OptionSetRecord setRecord) {
            var defaultItem = GetItemsFromSet(setRecord.Id).Where(x=> x.IsDefault);
            if (!defaultItem.Any()) {
                return;
            }
            foreach (var item in defaultItem) {
                item.IsDefault = false;
            }
            _optionSetRepository.Update(setRecord);
        }

        #region OptionItem&OptionSet Methods
        public bool EditItem(int id, OptionItemRecord optionItem) {
            var optionSet = (from set in _optionSetRepository.Table
                             from item in _optionItemRepository.Table
                             where item.Id == id && item.OptionSetRecord == set
                             select set).SingleOrDefault();
            if (optionSet == null || optionSet.Id == 0) {
                return false;
            }
            var optionItemRecord = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItem.IsDefault) {
                ResetDefault(optionSet);
            }           
            optionItemRecord.Value = optionItem.Value;
            optionItemRecord.IsDefault = optionItem.IsDefault;
            _optionSetRepository.Update(optionSet);
            return true;
        }

        public bool CreateItem(string entityName, string fieldName, OptionItemRecord optionItem) {
            var setId = GetOptionSetId(entityName, fieldName);
            if (setId <= 0) {
                return false;
            }
            var optionSet = _optionSetRepository.Get(setId);
            if (optionItem.IsDefault) {
                ResetDefault(optionSet);
            }
            optionItem.OptionSetRecord = optionSet;
            _optionItemRepository.Create(optionItem);
            return true;
        }

        public bool DeleteItem(int id) {
            var optionItem = (from item in _optionItemRepository.Table
                             where item.Id == id
                             select item).SingleOrDefault();
            if (optionItem == null || optionItem.Id == 0) {
                return false;
            }
            _optionItemRepository.Delete(optionItem);
            return true;
        }

        public object GetItemsForField(string entityName, string fieldName) {
            var setId = GetOptionSetId(entityName, fieldName);
            if(setId <=0) {
                return null;
            }
            return setId <= 0 ? null
                : GetItemsFromSet(setId).Select(x => new { x.Id, x.IsDefault, x.Value });
        }

        public List<SelectListItem> GetItemsForField(int setId) {

            return (from i in GetItemsFromSet(setId)
                    select new SelectListItem {
                        Value = i.Id.ToString(),
                        Text = i.Value,
                        Selected = i.IsDefault
                    }).ToList();
        }

        public void DeleteOptionSetForField(string fieldName, string entityName) {
            var setId = GetOptionSetId(entityName, fieldName);
            if (setId <= 0) {
                return;
            }
            _optionSetRepository.Delete(_optionSetRepository.Get(setId));        
        }

        public int GetItemCountForField(int setId) {
            return GetItemsFromSet(setId).Length;
        }

        public int InitializeField(string fieldName, string[] labels, int defaultValue) {
            var optionSet = new OptionSetRecord {
                FieldName = fieldName,
                //OptionItemRecords = new List<OptionItemRecord>()
            };
            _optionSetRepository.Create(optionSet);
            var idIndex = 0;
            foreach (var label in labels) {
                idIndex++;
                var item = new OptionItemRecord {
                    Value = label,
                    IsDefault = (idIndex == defaultValue),
                    OptionSetRecord = optionSet
                };
                _optionItemRepository.Create(item);
                //optionSet.OptionItemRecords.Add(item);
            }         
            return optionSet.Id;
        }

        #endregion

        #region SelectedOption & SelectedOptionSet Methods
        public int CreateSelectedSet(string[] optionIds) {
            int[] selectedValues;
            try {
                selectedValues = Array.ConvertAll(optionIds, int.Parse);
            }
            catch (Exception ex) {
                Logger.Log(LogLevel.Error, ex , null);
                return -1;
            }
            var selectedSet = new SelectedOptionSetRecord {
                SelectedOptionRecords = new List<SelectedOptionRecord>()
            };
            try {
                foreach (var value in selectedValues) {
                    selectedSet.SelectedOptionRecords.Add(new SelectedOptionRecord {
                        OptionItem = _optionItemRepository.Get(value)
                    });
                }
            }
            catch (Exception ex) {
                Logger.Log(LogLevel.Error, ex, null);
                return -1;
            }
            _selectedOptionSetRepository.Create(selectedSet);
            return selectedSet.Id;
        }

        public void DeleteSet(int setId) {
            if (setId == 0) {
                return;
            }

            var setRecord = _selectedOptionSetRepository.Get(setId);
            if (setRecord == null || setRecord.Id == 0) {
                return;
            }
            _selectedOptionSetRepository.Delete(setRecord);
        }

        public int AlterSet(int setId, string[] optionIds) {
            DeleteSet(setId);
            return CreateSelectedSet(optionIds);
        }

        public string[] GetSelectedSet(int setId) {
            var setRecord = _selectedOptionSetRepository.Get(setId);
            if (setRecord == null || setRecord.Id == 0) {
                return null;
            }
            return (from option in setRecord.SelectedOptionRecords
                    select option.OptionItem.Id.ToString("D")).ToArray();
        }
        #endregion
    }
}