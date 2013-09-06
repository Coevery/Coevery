using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    /// <summary>
    /// Helps with producing delegates that can be safely invoked in the background or as an async callback
    /// </summary>
    public interface IDetachedDelegateBuilder : IDependency
    {
        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code in an async event handler code.
        /// You can add this method's result's Invoke delegate to an event handler.
        /// </summary>
        /// <example>
        /// Usage with WebClient:
        /// using (var wc = new WebClient())
        /// {
        ///     wc.DownloadDataCompleted += _taskFactory.BuildAsyncEventHandler<object, DownloadDataCompletedEventArgs>(
        ///         (sender, e) =>
        ///         {
        ///             ...
        ///         }).Invoke;
        ///     wc.DownloadDataAsync(...);
        /// }
        /// </example>
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="action"></param>
        /// <param name="catchExceptions"></param>
        /// <returns></returns>
        Action<TSender, TEventArgs> BuildAsyncEventHandler<TSender, TEventArgs>(Action<TSender, TEventArgs> action, bool catchExceptions = true)
            where TEventArgs : EventArgs;

        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code in background.
        /// 
        /// Use this when instantiating the Task class.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// and unobserved exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated action</returns>
        Action BuildBackgroundAction(Action action, bool catchExceptions = true);

        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code in background.
        /// 
        /// Use this when instantiating the Task class.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// and unobserved exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated action</returns>
        Action<object> BuildBackgroundAction(Action<object> action, bool catchExceptions = true);

        /// <summary>
        /// Encapsulates the specified function so that it can safely run Orchard code in background.
        /// 
        /// Use this when instantiating the Task class.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="function">The function to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// and unobserved exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated function</returns>
        Func<TResult> BuildBackgroundFunction<TResult>(Func<TResult> function, bool catchExceptions = true);

        /// <summary>
        /// Encapsulates the specified function so that it can safely run Orchard code in background.
        /// 
        /// Use this when instantiating the Task class.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="function">The function to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// and unobserved exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated function</returns>
        Func<object, TResult> BuildBackgroundFunction<TResult>(Func<object, TResult> function, bool catchExceptions = true);
    }
}
