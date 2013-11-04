namespace Coevery.Projections.Services {
    public interface ISortService : IDependency {
        void MoveUp(int sortId);
        void MoveDown(int sortId);
    }
}
