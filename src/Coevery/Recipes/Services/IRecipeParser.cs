using Coevery.Recipes.Models;

namespace Coevery.Recipes.Services {
    public interface IRecipeParser : IDependency {
        Recipe ParseRecipe(string recipeText);
    }
}
