using System.Collections.Generic;
using Coevery.Metadata.Services;
using Coevery.Metadata.Settings;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Metadata.ViewModels {
    public class EditPlacementViewModel {
        public ContentTypeDefinition ContentTypeDefinition { get; set; }
        public PlacementSettings[] PlacementSettings { get; set; }
        public List<DriverResultPlacement> AllPlacements { get; set; }
    }
}