using System;
using Coevery.ContentManagement.Drivers;
using Coevery.Environment.Extensions;
using Coevery.Localization;
using Coevery.Scripting.CSharp.Models;
using Coevery.Scripting.CSharp.Services;
using Coevery.Scripting.CSharp.Settings;

namespace Coevery.Scripting.CSharp.Drivers {
    [CoeveryFeature("Coevery.Scripting.CSharp.Validation")]
    public class ScriptValidationPartDriver : ContentPartDriver<ScriptValidationPart> {
        private readonly ICSharpService _csharpService;
        private readonly ICoeveryServices _coeveryServices;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ScriptValidationPartDriver(
            ICSharpService csharpService, 
            ICoeveryServices coeveryServices, 
            IWorkContextAccessor workContextAccessor) {
            _csharpService = csharpService;
            _coeveryServices = coeveryServices;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ICoeveryServices Services { get; set; }

        protected override string Prefix {
            get { return "SpamFilter"; }
        }

        protected override DriverResult Editor(ScriptValidationPart part, Coevery.ContentManagement.IUpdateModel updater, dynamic shapeHelper) {
            var script = part.Settings.GetModel<ScriptValidationPartSettings>().Script;

            if (!String.IsNullOrWhiteSpace(script)) {

                _csharpService.SetParameter("Services", _coeveryServices);
                _csharpService.SetParameter("ContentItem", (dynamic)part.ContentItem);
                _csharpService.SetParameter("WorkContext", _workContextAccessor.GetContext());
                _csharpService.SetFunction("T", (Func<string, string>)(x => T(x).Text));
                _csharpService.SetFunction("AddModelError", (Action<string>)(x => updater.AddModelError("Script", T(x))));

                _csharpService.Run(script);
            }

            return Editor(part, shapeHelper);
        }
    }
}