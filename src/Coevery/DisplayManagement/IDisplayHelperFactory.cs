using System.Web.Mvc;

namespace Coevery.DisplayManagement {
    /// <summary>
    /// Used to create a dynamic, contextualized Display object to dispatch shape rendering
    /// </summary>
    public interface IDisplayHelperFactory : IDependency {
        dynamic CreateHelper(ViewContext viewContext, IViewDataContainer viewDataContainer);
    }
}