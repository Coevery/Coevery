using System;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Exceptions;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Settings;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    // The implementation of action and func passing below is quite ugly, but this is the only way to make it DRY. There is no
    // real performance impact.
    public class DetachedDelegateBuilder : IDetachedDelegateBuilder
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public ILogger Logger { get; set; }

        private class TaskContext
        {
            public string CurrentCulture { get; private set; }
            public ISite CurrentSite { get; private set; }
            public IUser CurrentUser { get; set; }
            public HttpContextBase HttpContext { get; private set; }

            public TaskContext(WorkContext workContext)
            {
                CurrentCulture = workContext.CurrentCulture;
                CurrentSite = workContext.CurrentSite;
                CurrentUser = workContext.CurrentUser;
                //HttpContext = new HttpContextPlaceholder();
                HttpContext = workContext.HttpContext;
            }

            public WorkContext Transcribe(WorkContext workContext)
            {
                workContext.CurrentCulture = CurrentCulture;
                workContext.CurrentSite = CurrentSite;
                workContext.CurrentUser = CurrentUser;
                workContext.HttpContext = HttpContext;

                return workContext;
            }
        }

        public DetachedDelegateBuilder(
            IWorkContextAccessor workContextAcessor
            )
        {
            _workContextAccessor = workContextAcessor;

            Logger = NullLogger.Instance; // Constructor injection of ILogger fails
        }

        public Action<TSender, TEventArgs> BuildAsyncEventHandler<TSender, TEventArgs>(Action<TSender, TEventArgs> action, bool catchExceptions = true)
            where TEventArgs : EventArgs
        {
            var subAction = BuildBackgroundAction(
                                (o) =>
                                {
                                    var eventParams = (Tuple<TSender, TEventArgs>)o;

                                    action(eventParams.Item1, eventParams.Item2);
                                }, catchExceptions);

            return (sender, e) =>
                    {
                        subAction(new Tuple<TSender, TEventArgs>(sender, e));
                    };
        }

        public Action BuildBackgroundAction(Action action, bool catchExceptions = true)
        {
            var subAction = BuildBackgroundAction(
                                (o) =>
                                {
                                    action();
                                }, catchExceptions);
            return () =>
                    {
                        subAction(new object());
                    };
        }

        public Action<object> BuildBackgroundAction(Action<object> action, bool catchExceptions = true)
        {
            var subFunction = BuildBackgroundFunction<int>(
                                (o) =>
                                {
                                    action(o);
                                    return 1;
                                }, catchExceptions);

            return (state) =>
                    {
                        subFunction(state);
                    };
        }

        public Func<TResult> BuildBackgroundFunction<TResult>(Func<TResult> function, bool catchExceptions = true)
        {
            var subFunction = BuildBackgroundFunction<TResult>(
                                (o) =>
                                {
                                    return function();
                                }, catchExceptions);

            return () =>
                    {
                        return subFunction(new object());
                    };
        }

        public Func<object, TResult> BuildBackgroundFunction<TResult>(Func<object, TResult> function, bool catchExceptions = true)
        {
            var taskContext = new TaskContext(_workContextAccessor.GetContext());

            return (state) =>
            {
                using (var scope = _workContextAccessor.CreateWorkContextScope())
                {
                    taskContext.Transcribe(_workContextAccessor.GetContext());

                    if (catchExceptions)
                    {
                        try
                        {
                            return function(state);
                        }
                        catch (Exception ex)
                        {
                            if (ex.IsFatal()) throw;
                            Logger.Error(ex, "Background task failed with exception " + ex.Message);
                            return default(TResult);
                        }
                    }
                    else
                    {
                        return function(state);
                    }
                }
            };
        }
    }
}
