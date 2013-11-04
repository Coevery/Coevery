using System.Xml.Linq;

namespace Coevery.Recipes.Models {
    public class RecipeStep {
        public string Name { get; set; }
        public XElement Step { get; set; }
    }
}