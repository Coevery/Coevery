
namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Describes a class that initially can be modified but after frozen not anymore
    /// </summary>
    public interface IFreezable
    {
        /// <summary>
        /// Checks if the object can be modified or not
        /// </summary>
        bool IsFrozen { get; }

        /// <summary>
        /// Prevents further modification of the object
        /// </summary>
        void Freeze();
    }
}
