using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Utility.Extensions;

namespace Coevery.Entities.ViewModels {
    public class EditPartViewModel {
        public EditPartViewModel() {
            Fields = new List<EditPartFieldViewModel>();
            Settings = new SettingsDictionary();
        }

        public EditPartViewModel(ContentPartDefinition contentPartDefinition) {
            Name = contentPartDefinition.Name;
            Fields = contentPartDefinition.Fields.Select((f, i) => new EditPartFieldViewModel(i, f) { Part = this }).ToList();
            Settings = contentPartDefinition.Settings;
            _Definition = contentPartDefinition;
        }

        public string Prefix { get { return "PartDefinition"; } }
        public string Name { get; set; }
        private string _displayName;
        [Required]
        public string DisplayName {
            get { return !string.IsNullOrWhiteSpace(_displayName) ? _displayName : Name.RemovePartSuffix().CamelFriendly(); }
            set { _displayName = value; }
        }

        public string Description {
            get { return Settings.ContainsKey("ContentPartSettings.Description") ? Settings["ContentPartSettings.Description"] : null; }
            set { Settings["ContentPartSettings.Description"] = value;}
        }

        public IEnumerable<TemplateViewModel> Templates { get; set; }
        public IEnumerable<EditPartFieldViewModel> Fields { get; set; }
        public SettingsDictionary Settings { get; set; }
        public ContentPartDefinition _Definition { get; private set; }
    }
}