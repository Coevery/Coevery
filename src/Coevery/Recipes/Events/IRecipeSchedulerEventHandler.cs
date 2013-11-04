using Coevery.Events;

namespace Coevery.Recipes.Events {
    public interface IRecipeSchedulerEventHandler : IEventHandler  {
        void ExecuteWork(string executionId);
    }
}
