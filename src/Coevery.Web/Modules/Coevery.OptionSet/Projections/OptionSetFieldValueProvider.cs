using System.Linq;
using Coevery.Entities.Services;
using Coevery.OptionSet.Fields;
using Coevery.OptionSet.Services;
using Coevery.ContentManagement;

namespace Coevery.OptionSet.Projections {

    public class OptionSetFieldValueProvider : ContentFieldValueProvider<OptionSetField> {
        private readonly IOptionSetService _optionSetService;

        public OptionSetFieldValueProvider(IOptionSetService optionSetService) {
            _optionSetService = optionSetService;
        }

        public override object GetValue(ContentItem contentItem, ContentField field) {
            var optionItems = _optionSetService.GetOptionItemsForContentItem(contentItem.VersionRecord.Id, field.Name).ToList();

            var value = string.Join(", ", optionItems.Select(t => t.Name).ToArray());
            return value;
        }
    }
}