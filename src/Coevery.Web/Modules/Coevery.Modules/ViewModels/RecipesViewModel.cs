using System.Collections.Generic;
using Coevery.Modules.Models;
using Coevery.Recipes.Models;

namespace Coevery.Modules.ViewModels {
    public class RecipesViewModel {
        public IEnumerable<ModuleRecipesViewModel> Modules { get; set; }
    }

    public class ModuleRecipesViewModel {
        public ModuleEntry Module { get; set; }
        public IEnumerable<Recipe> Recipes { get; set; } 
    }
}