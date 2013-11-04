using Coevery.Recipes.Models;

namespace Coevery.Recipes.Services {
    public interface IRecipeManager : IDependency {
        string Execute(Recipe recipe);
    }
}
