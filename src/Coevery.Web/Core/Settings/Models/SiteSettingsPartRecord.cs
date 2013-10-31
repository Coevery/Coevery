using System;
using Coevery.ContentManagement.Records;
using Coevery.Settings;

namespace Coevery.Core.Settings.Models {
    public class SiteSettingsPartRecord : ContentPartRecord {
        public const int DefaultPageSize = 10;

        public SiteSettingsPartRecord() {
            PageSize = DefaultPageSize;
        }

        public virtual string SiteSalt { get; set; }

        public virtual string SiteName { get; set; }

        public virtual string SuperUser { get; set; }

        public virtual string PageTitleSeparator { get; set; }

        public virtual string HomePage { get; set; }

        public virtual string SiteCulture { get; set; }

        public virtual ResourceDebugMode ResourceDebugMode { get; set; }

        public virtual int PageSize { get; set; }

        public virtual string SiteTimeZone { get; set; }
    }
}