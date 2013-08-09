using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Entities.Records;
using Orchard;

namespace Coevery.Entities.Services {
    public interface IOptionItemService : IDependency {
        bool EditItem(int id, OptionItemRecord optionItem);
        bool CreateItem(string entityName, string fieldName, OptionItemRecord optionItem);
        bool DeleteItem(int id);
        object GetItemsForField(string entityName, string fieldName);
        List<SelectListItem> GetItemsForField(int fieldId);
        void DeleteItemsForField(string fieldName, string entityName);
        int GetItemCountForField(int fieldId);
        int InitializeField(string entityName, string fieldName, string[] labels, int defaultValue);
    }
}