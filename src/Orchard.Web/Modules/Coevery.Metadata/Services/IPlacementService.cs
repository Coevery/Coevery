using System.Collections.Generic;
using Orchard;

namespace Coevery.Metadata.Services {
    public interface IPlacementService : IDependency {
        IEnumerable<DriverResultPlacement> GetDisplayPlacement(string contentType);
        IEnumerable<DriverResultPlacement> GetEditorPlacement(string contentType);
        IEnumerable<string> GetZones();
    }
}