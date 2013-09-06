using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Coevery.Relationship.Models {
    public class ReferenceUpdateModel:IUpdateModel {
        public ReferenceFieldSettings Setting { get; set; }

        public ReferenceUpdateModel(ReferenceFieldSettings viewModel) {
            Setting = viewModel;
        }

        public void AddModelError(string key, LocalizedString message) {
            throw new FieldAccessException(key + ":" + message);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            if (model == null || !(model is ReferenceFieldSettings)) {
                return false;
            }
            var temp = model as ReferenceFieldSettings;
            temp.AlwaysInLayout = Setting.AlwaysInLayout;
            temp.ContentTypeName = Setting.ContentTypeName;
            temp.DisplayAsLink = Setting.DisplayAsLink;
            temp.HelpText = Setting.HelpText;
            temp.IsAudit = Setting.IsAudit;
            temp.IsSystemField = Setting.IsSystemField;
            temp.QueryId = 0;
            temp.ReadOnly = Setting.ReadOnly;
            temp.RelationshipId = Setting.RelationshipId;
            temp.RelationshipName = Setting.RelationshipName;
            temp.Required = Setting.Required;
            return true;
        }
    }
}