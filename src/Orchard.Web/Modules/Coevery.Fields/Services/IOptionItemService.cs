using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Fields.Records;
using Orchard;

namespace Coevery.Fields.Services {
    public interface IOptionItemService : IDependency {
        bool EditItem(int id, OptionItemRecord optionItem);
        bool CreateItem(string entityName, string fieldName, OptionItemRecord optionItem);
        bool DeleteItem(int id);
        object GetItemsForField(string entityName, string fieldName);
        List<SelectListItem> GetItemsForField(int fieldId);
        void DeleteOptionSetForField(string fieldName, string entityName);
        int GetItemCountForField(int setId);
        int InitializeField(string fieldName, string[] labels, int defaultValue);

        int CreateSelectedSet(string[] optionIds);
        int AlterSet(int setId, string[] optionIds);
        void DeleteSet(int setId);
        string[] GetSelectedSet(int setId);
    }
}