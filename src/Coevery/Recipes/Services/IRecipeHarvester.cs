using System.Collections.Generic;
using Coevery.Recipes.Models;

namespace Coevery.Recipes.Services {
    public interface IRecipeHarvester : IDependency {
        IEnumerable<Recipe> HarvestRecipes(string extensionId);
    }
}
