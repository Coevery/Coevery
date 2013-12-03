namespace Coevery.Alias.Implementation.Updater {
    public interface IAliasUpdateCursor : ISingletonDependency {
        int Cursor { get; set; } 
    }
}