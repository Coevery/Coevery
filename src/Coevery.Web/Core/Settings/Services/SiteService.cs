using System;
using System.Linq;
using JetBrains.Annotations;
using Coevery.Caching;
using Coevery.Core.Settings.Models;
using Coevery.Data;
using Coevery.Logging;
using Coevery.ContentManagement;
using Coevery.Settings;

namespace Coevery.Core.Settings.Services {
    [UsedImplicitly]
    public class SiteService : ISiteService {
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;

        public SiteService(
            IContentManager contentManager,
            ICacheManager cacheManager) {
            _contentManager = contentManager;
            _cacheManager = cacheManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ISite GetSiteSettings() {
            var siteId = _cacheManager.Get("SiteId", ctx => {
                var site = _contentManager.Query("Site")
                    .List()
                    .FirstOrDefault();

                if (site == null) {
                    site = _contentManager.Create<SiteSettingsPart>("Site", item => {
                        item.SiteSalt = Guid.NewGuid().ToString("N");
                        item.SiteName = "My Coevery Project Application";
                        item.PageTitleSeparator = " - ";
                        item.SiteTimeZone = TimeZoneInfo.Local.Id;
                    }).ContentItem;
                }

                return site.Id;
            });

            return _contentManager.Get<ISite>(siteId, VersionOptions.Published);
        }
    }
}