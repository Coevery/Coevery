using System.Web.Mvc;
using Coevery.Core.Containers.Models;

namespace Coevery.Core.Containers.ViewModels {
    public class ContainerViewModel {
        public ContainerPart Part { get; set; }
        public SelectList AvailableContainables { get; set; }
    }
}