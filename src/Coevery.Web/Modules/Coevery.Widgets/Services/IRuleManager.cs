namespace Coevery.Widgets.Services {
    public interface IRuleManager : IDependency {
        bool Matches(string expression);
    }
}