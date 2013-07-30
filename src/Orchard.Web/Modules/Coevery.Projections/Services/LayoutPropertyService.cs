using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.Services
{
    public class LayoutPropertyService:ILayoutPropertyService
    {

        private readonly IContentManager _contentManager;
        public LayoutPropertyService(IContentManager contentManager)
        {
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
        }
        public Localizer T { get; set; }

        public LayoutPropertyPart CreateLayoutProperty(LayoutPropertyRecord layoutPropertyRecord) {
            var layoutProperty = GetLayoutPropertyByQueryid(layoutPropertyRecord.QueryPartRecord_id);
            if (layoutProperty == null)
            {
                var contentItem = _contentManager.New("LayoutProperty");
                layoutProperty = contentItem.As<LayoutPropertyPart>();
                layoutProperty.VisableTo = layoutPropertyRecord.VisableTo;
                layoutProperty.PageRowCount = layoutPropertyRecord.PageRowCount;
                layoutProperty.QueryPartRecord_id = layoutPropertyRecord.QueryPartRecord_id;
                _contentManager.Create(contentItem);
            }
            else {
                layoutProperty.VisableTo = layoutPropertyRecord.VisableTo;
                layoutProperty.PageRowCount = layoutPropertyRecord.PageRowCount;
                _contentManager.Publish(layoutProperty.ContentItem);
            }
            return layoutProperty;
        }

        public LayoutPropertyPart GetLayoutProperty(int id)
        {
            return _contentManager.Get<LayoutPropertyPart>(id);
        }

        public LayoutPropertyPart GetLayoutPropertyByQueryid(int queryid)
        {
            return _contentManager.Query<LayoutPropertyPart>().List().FirstOrDefault(c => c.QueryPartRecord_id == queryid);
        }

        public void DeleteLayoutPropertyPart(int id)
        {
            var layoutProperty = _contentManager.Get(id);

            if (layoutProperty != null)
            {
                _contentManager.Remove(layoutProperty);
            }
        }
    }
}