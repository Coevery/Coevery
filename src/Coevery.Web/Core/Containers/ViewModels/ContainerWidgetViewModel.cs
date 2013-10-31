using System.Web.Mvc;
using Coevery.Core.Containers.Models;

namespace Coevery.Core.Containers.ViewModels {
    public class ContainerWidgetViewModel {
        public bool UseFilter { get; set; }
        public SelectList AvailableContainers { get; set; }
        public ContainerWidgetPart Part { get; set; }
    }
}