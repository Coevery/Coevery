using System.Collections.Generic;

namespace Coevery.Workflows.Services {
    public interface IActivitiesManager : IDependency {
        IEnumerable<IActivity> GetActivities();
        IActivity GetActivityByName(string name);
    }

}