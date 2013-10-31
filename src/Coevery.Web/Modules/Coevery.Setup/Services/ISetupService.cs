using System.Collections.Generic;
using Coevery.Environment.Configuration;
using Coevery.Recipes.Models;

namespace Coevery.Setup.Services {
    public interface ISetupService : IDependency {
        ShellSettings Prime();
        IEnumerable<Recipe> Recipes();
        string Setup(SetupContext context);
    }
}