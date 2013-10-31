using Coevery.Events;
using Coevery.Recipes.Models;

namespace Coevery.Recipes.Events {
    public interface IRecipeExecuteEventHandler : IEventHandler {
        void ExecutionStart(string executionId, Recipe recipe);
        void RecipeStepExecuting(string executionId, RecipeContext context);
        void RecipeStepExecuted(string executionId, RecipeContext context);
        void ExecutionComplete(string executionId);
        void ExecutionFailed(string executionId);
    }
}