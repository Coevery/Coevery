using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Core.Common.ViewModels;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Utility.Extensions;

namespace Coevery.Core.Common.Settings {
    public class TextFieldSettingsEvents : ContentDefinitionEditorEventsBase {
        private readonly ICoeveryServices _coeveryServices;
        private readonly Func<IShapeTableLocator> _shapeTableLocator;

        public TextFieldSettingsEvents(ICoeveryServices coeveryServices, Func<IShapeTableLocator> shapeTableLocator) {
            _coeveryServices = coeveryServices;
            _shapeTableLocator = shapeTableLocator;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "TextField") {
                var shapeTable = _shapeTableLocator().Lookup(_coeveryServices.WorkContext.CurrentTheme.Id);
                var flavors = shapeTable.Bindings.Keys
                    .Where(x => x.StartsWith("Body_Editor__", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Substring("Body_Editor__".Length))
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .Select(x => x[0].ToString(CultureInfo.InvariantCulture).ToUpper() + x.Substring(1) )
                    .Select(x => x.CamelFriendly())
                    ;


                var model = new TextFieldSettingsEventsViewModel {
                    Settings = definition.Settings.GetModel<TextFieldSettings>(),
                    Flavors = flavors.ToArray()
                };
                    
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "TextField") {
                yield break;
            }

            var model = new TextFieldSettingsEventsViewModel {
                Settings = new TextFieldSettings()
            };

            if (updateModel.TryUpdateModel(model, "TextFieldSettingsEventsViewModel", null, null)) {
                builder.WithSetting("TextFieldSettings.Flavor", model.Settings.Flavor);
                builder.WithSetting("TextFieldSettings.Hint", model.Settings.Hint);
                builder.WithSetting("TextFieldSettings.Required", model.Settings.Required.ToString(CultureInfo.InvariantCulture));

                yield return DefinitionTemplate(model);
            }
        }
    }
}