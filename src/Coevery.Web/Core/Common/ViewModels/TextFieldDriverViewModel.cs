using Coevery.Core.Common.Fields;
using Coevery.Core.Common.Settings;

namespace Coevery.Core.Common.ViewModels {
    public class TextFieldDriverViewModel {
        public TextField Field { get; set; }
        public string Text { get; set; }
        public TextFieldSettings Settings { get; set; } 
    }
}