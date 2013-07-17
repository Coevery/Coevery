using JetBrains.Annotations;
using Orchard.ContentManagement.Drivers;
using Orchard.CulturePicker.Models;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Drivers {
    [UsedImplicitly]
    public class CulturePickerPartDriver : ContentPartDriver<CulturePickerPart> {
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CulturePickerPartDriver(ICultureManager cultureManager, IWorkContextAccessor workContextAccessor) {
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        protected override DriverResult Display(CulturePickerPart part, string displayType, dynamic shapeHelper) {
            part.AvailableCultures = _cultureManager.ListCultures();
            part.UserCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);

            return ContentShape("Parts_CulturePicker", () => shapeHelper.Parts_CulturePicker(AvailableCultures: part.AvailableCultures, UserCulture: part.UserCulture));
        }
    }
}