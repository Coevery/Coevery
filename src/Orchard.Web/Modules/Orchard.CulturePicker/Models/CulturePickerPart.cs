using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Orchard.CulturePicker.Models {
    public class CulturePickerPart : ContentPart {
        public IEnumerable<string> AvailableCultures { get; set; }

        public string UserCulture { get; set; }
    }
}