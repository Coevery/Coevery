using Coevery.Recipes.Models;

namespace Coevery.Recipes.Services {
    public interface IRecipeStepQueue : ISingletonDependency {
        void Enqueue(string executionId, RecipeStep step);
        RecipeStep Dequeue(string executionId);
    }
}
