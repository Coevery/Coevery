using System;
using System.Collections.Generic;
using System.Web.Routing;
using Coevery.ContentManagement.Handlers;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.FileSystems.VirtualPath;
using Coevery.Logging;
using Coevery.Environment.Extensions;
using Four2n.Orchard.MiniProfiler.Services;

namespace Coevery.ContentManagement
{
    [CoeverySuppressDependency("Coevery.ContentManagement.DefaultContentDisplay")]
    public class ProfilingContentDisplay : IContentDisplay {
        private readonly IProfilerService _profiler;
        private readonly DefaultContentDisplay _innerContentDisplay;

        public ProfilingContentDisplay(
            Lazy<IEnumerable<IContentHandler>> handlers,
            IShapeFactory shapeFactory,
            Lazy<IShapeTableLocator> shapeTableLocator, 
            RequestContext requestContext,
            IVirtualPathProvider virtualPathProvider,
            IWorkContextAccessor workContextAccessor,
            IProfilerService profiler)
        {

            _innerContentDisplay = new DefaultContentDisplay(
                handlers,
                shapeFactory,
                shapeTableLocator,
                requestContext,
                virtualPathProvider,
                workContextAccessor);

            _profiler = profiler;
        }

        public ILogger Logger
        {
            get { return _innerContentDisplay.Logger; }
            set { _innerContentDisplay.Logger = value; }
        }

        public dynamic BuildDisplay(IContent content, string displayType, string groupId) {
            var contentKey = "ContentDisplay:" + content.Id.ToString();
            _profiler.StepStart(contentKey, String.Format("Content Display: {0} ({1})", content.Id, displayType));
            var result = _innerContentDisplay.BuildDisplay(content, displayType, groupId);
            _profiler.StepStop(contentKey);
            return result;
        }

        public dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupInfoId) {
            var contentKey = "ContentUpdate:" + content.Id.ToString();
            _profiler.StepStart(contentKey, String.Format("Content Update: {0}", content.Id));
            var result = _innerContentDisplay.UpdateEditor(content, updater, groupInfoId);
            _profiler.StepStop(contentKey);
            return result;
        }


        public dynamic BuildEditor(IContent content, string displayType = "", string groupId = "")
        {
            var contentKey = "ContentEditor:" + content.Id.ToString();
            _profiler.StepStart(contentKey, String.Format("Content Editor: {0}", content.Id));
            var result = _innerContentDisplay.BuildEditor(content, displayType, groupId);
            _profiler.StepStop(contentKey);
            return result;
        }
    }
}
