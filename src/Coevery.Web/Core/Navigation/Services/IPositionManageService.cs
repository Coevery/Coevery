using Coevery.Core.Navigation.ViewModels;

namespace Coevery.Core.Navigation.Services {
    public interface IPositionManageService : IDependency {
        PositionTreeModel ParseMenuPostion(string position, int perspectiveId);
    }
}