using Coevery.Recipes.Models;

namespace Coevery.Recipes.Services {
    public interface IRecipeHandler : IDependency {
        void ExecuteRecipeStep(RecipeContext recipeContext);
    }
}
