using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using NHibernate;
using NHibernate.Linq;
using Coevery.Translations.ViewModels;
using Coevery.Translations.Models;
using Coevery.Data;
using Coevery.UI.Admin.Notification;
using Coevery.Localization;
using Coevery.Localization.Services;
using Coevery.Caching;
using Coevery.Environment.Configuration;
using Coevery.UI.Notify;

namespace Coevery.Translations.Services
{
    public interface ILocalizationService : IDependency
    {
        CultureDetailsViewModel GetCultureDetailsViewModel(string culture);
        byte[] GetZipBytes(string culture);
        CultureIndexViewModel GetCultures();
        void UpdateTranslation(int id, string culture, string value);
        void RemoveTranslation(int id, string culture);
        IEnumerable<StringEntry> GetTranslationsFromZip(Stream stream);
        bool IsCultureAllowed(string culture);
        IEnumerable<string> GetTranslatedCultures();
        void ResetCache();
        IEnumerable<StringEntry> TranslateFile(string path, string content, string culture);
        void SaveStringsToDatabase(IEnumerable<StringEntry> strings, bool overwrite);
        CultureGroupDetailsViewModel GetModules(string culture);
        CultureGroupDetailsViewModel GetTranslations(string culture, string path);
        IEnumerable<StringEntry> GetTranslations(string culture);
        void SavePoFilesToDisk(string culture);
        void SavePoFilesToDisk();
        CultureGroupDetailsViewModel Search(string culture, string querystring);
    }

    public class LocalizationService : ILocalizationService, INotificationProvider
    {
        private readonly IWorkContextAccessor _wca;
        private readonly ISessionLocator _sessionLocator;
        private readonly ICultureManager _cultureManager;
        public Localizer T { get; set; }
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly ICoeveryServices _services;
        private readonly ShellSettings _shellSettings;
        private readonly IRepository<TranslationRecord> _translationRepository;
        private readonly IRepository<LocalizableStringRecord> _localizableStringRepository;

        public LocalizationService(IWorkContextAccessor wca,
            ISessionLocator sessionLocator,
            ICultureManager cultureManager,
            ICacheManager cacheManager, ISignals signals, ICoeveryServices services, ShellSettings shellSettings,
            IRepository<TranslationRecord> translationRepository,
            IRepository<LocalizableStringRecord> localizableStringRepository)
        {
            _sessionLocator = sessionLocator;
            _shellSettings = shellSettings;
            _services = services;
            _signals = signals;
            T = NullLocalizer.Instance;
            _wca = wca;
            _cultureManager = cultureManager;
            _cacheManager = cacheManager;
            _translationRepository = translationRepository;
            _localizableStringRepository = localizableStringRepository;
        }

        private string DataTablePrefix()
        {
            if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix)) return string.Empty;
            return _shellSettings.DataTablePrefix + "_";
        }

        public CultureDetailsViewModel GetCultureDetailsViewModel(string culture)
        {
            var model = new CultureDetailsViewModel { Culture = culture };
            var query = _localizableStringRepository.Table.AsEnumerable();
            var currentPath = "";
            var group = default(CultureDetailsViewModel.TranslationGroupViewModel);
            foreach (LocalizableStringRecord s in query)
            {
                if (s.Path != currentPath)
                {
                    group = new CultureDetailsViewModel.TranslationGroupViewModel
                    {
                        Path = String.Format(s.Path, culture)
                    };
                    if (!group.Path.Contains(culture))
                        throw new Exception("Something went wrong: the culture is not included in the path.");
                    model.Groups.Add(group);
                    currentPath = s.Path;
                }
                if (group != null)
                {
                    var translation = s.Translations.Where(t => t.Culture == culture).FirstOrDefault();
                    group.Translations.Add(new CultureDetailsViewModel.TranslationViewModel
                    {
                        Context = s.Context,
                        Key = s.StringKey,
                        OriginalString = s.OriginalLanguageString,
                        LocalString = translation != null ? translation.Value : null
                    });
                }
            }

            return model;
        }



        public CultureGroupDetailsViewModel GetModules(string culture)
        {
            var model = new CultureGroupDetailsViewModel { Culture = culture };
            var session = _sessionLocator.For(typeof(LocalizableStringRecord));
            var paths = session.CreateSQLQuery(
                string.Format(@"  SELECT Localizable.Path,
                    COUNT(Localizable.Id) AS TotalCount,
                    COUNT(Translation.Id) AS TranslatedCount
                FROM {0}Coevery_Translations_LocalizableStringRecord AS Localizable
                LEFT OUTER JOIN {0}Coevery_Translations_TranslationRecord AS Translation
                    ON Localizable.Id = Translation.LocalizableStringRecord_id
                    AND Translation.Culture = :culture
                GROUP BY Localizable.Path
                ORDER BY Localizable.Path", DataTablePrefix()))
            .AddScalar("Path", NHibernateUtil.String)
            .AddScalar("TotalCount", NHibernateUtil.Int32)
            .AddScalar("TranslatedCount", NHibernateUtil.Int32)
            .SetParameter("culture", culture);
            model.Groups = paths.List<object[]>()
                .Select(t => new CultureGroupDetailsViewModel.TranslationGroup
                {
                    Path = (string)t[0],
                    TotalCount = (int)t[1],
                    TranslationCount = (int)t[2],
                }).ToList();
            return model;
        }

        public CultureGroupDetailsViewModel GetTranslations(string culture, string path)
        {
            var model = new CultureGroupDetailsViewModel { Culture = culture };
            var session = _sessionLocator.For(typeof(LocalizableStringRecord));
            // haalt alle mogelijke strings en description en hun vertaling in culture op
            var paths = session.CreateSQLQuery(
                    string.Format(@"  SELECT Localizable.Id,
                    Localizable.StringKey,
                    Localizable.Context,
                    Localizable.OriginalLanguageString,
                    Translation.Value
                FROM {0}Coevery_Translations_LocalizableStringRecord AS Localizable
                LEFT OUTER JOIN {0}Coevery_Translations_TranslationRecord AS Translation
                    ON Localizable.Id = Translation.LocalizableStringRecord_id
                    AND Translation.Culture = :culture
                WHERE Localizable.Path = :path", DataTablePrefix()))
                .AddScalar("Id", NHibernateUtil.Int32)
                .AddScalar("StringKey", NHibernateUtil.String)
                .AddScalar("Context", NHibernateUtil.String)
                .AddScalar("OriginalLanguageString", NHibernateUtil.String)
                .AddScalar("Value", NHibernateUtil.String)
                .SetParameter("culture", culture)
                .SetParameter("path", path);

            model.CurrentGroupTranslations = paths.List<object[]>()
                .Select(t => new CultureGroupDetailsViewModel.TranslationViewModel
                {
                    Id = (int)t[0],
                    GroupPath = path,
                    Key = (string)t[1],
                    Context = (string)t[2],
                    OriginalString = (string)t[3],
                    LocalString = (string)t[4]
                })
                .ToList()
                .GroupBy(t => t.Context);

            return model;
        }

        public CultureGroupDetailsViewModel Search(string culture, string querystring)
        {
            var model = new CultureGroupDetailsViewModel { Culture = culture };
            var session = _sessionLocator.For(typeof(LocalizableStringRecord));
            // haalt alle mogelijke strings en description en hun vertaling in culture op
            var paths = session.CreateSQLQuery(
                string.Format(@"  SELECT Localizable.Id,
                    Localizable.StringKey,
                    Localizable.Context,
                    Localizable.OriginalLanguageString,
                    Translation.Value,
                    Localizable.Path
                FROM {0}Coevery_Translations_LocalizableStringRecord AS Localizable
                LEFT OUTER JOIN {0}Coevery_Translations_TranslationRecord AS Translation
                    ON Localizable.Id = Translation.LocalizableStringRecord_id
                    AND Translation.Culture = :culture
                WHERE Localizable.OriginalLanguageString LIKE :query 
                    OR Translation.Value LIKE :query", DataTablePrefix()))
                .AddScalar("Id", NHibernateUtil.Int32)
                .AddScalar("StringKey", NHibernateUtil.String)
                .AddScalar("Context", NHibernateUtil.String)
                .AddScalar("OriginalLanguageString", NHibernateUtil.String)
                .AddScalar("Value", NHibernateUtil.String)
                .AddScalar("Path", NHibernateUtil.String)
                .SetParameter("culture", culture)
                .SetParameter("query", "%" + querystring + "%");
            model.CurrentGroupTranslations = paths.List<object[]>()
                .Select(t => new CultureGroupDetailsViewModel.TranslationViewModel
                {
                    Id = (int)t[0],
                    GroupPath = (string)t[5],
                    Key = (string)t[1],
                    Context = (string)t[2],
                    OriginalString = (string)t[3],
                    LocalString = (string)t[4]
                }).ToList().GroupBy(t => t.Context);
            return model;
        }

        public IEnumerable<StringEntry> GetTranslations(string culture)
        {
            var session = _sessionLocator.For(typeof(LocalizableStringRecord));
            // haalt alle mogelijke strings en description en hun vertaling in culture op
            //_localizableStringRepository.Table.
            var paths = session.CreateSQLQuery(
              string.Format(@"  SELECT 
              Localizable.StringKey,
              Localizable.Context,
              Translation.Value
          FROM {0}Coevery_Translations_LocalizableStringRecord AS Localizable
          INNER JOIN {0}Coevery_Translations_TranslationRecord AS Translation
              ON Localizable.Id = Translation.LocalizableStringRecord_id
              AND Translation.Culture = :culture", DataTablePrefix()))
              .AddScalar("StringKey", NHibernateUtil.String)
              .AddScalar("Context", NHibernateUtil.String)
              .AddScalar("Value", NHibernateUtil.String)
              .SetParameter("culture", culture);
            return paths.List<object[]>()
                .Select(t => new StringEntry
                {
                    Key = (string)t[0],
                    Context = (string)t[1],
                    Translation = (string)t[2]
                }).ToList();
        }


        public IEnumerable<StringEntry> TranslateFile(string path, string content, string culture)
        {
            string currentContext = null;
            string currentOriginal = null;
            string currentId = null;
            using (var textStream = new StringReader(content))
            {
                string line;
                while ((line = textStream.ReadLine()) != null)
                {
                    if (line.StartsWith("#: "))
                    {
                        currentContext = line.Substring(3);
                    }
                    if (line.StartsWith("msgctxt "))
                    {
                        currentContext = line.Substring(8);
                    }
                    else if (line.StartsWith("#| msgid \""))
                    {
                        currentId = ImportPoText(line.Substring(10, line.Length - 11));
                    }
                    else if (line.StartsWith("msgid \""))
                    {
                        currentOriginal = ImportPoText(line.Substring(7, line.Length - 8));
                    }
                    else if (line.StartsWith("msgstr \""))
                    {
                        var context = currentContext;
                        var translation = ImportPoText(line.Substring(8, line.Length - 9));
                        if (!string.IsNullOrEmpty(translation))
                        {
                            yield return new StringEntry
                            {
                                Context = context,
                                Path = path,
                                Culture = culture,
                                Key = currentId,
                                English = currentOriginal,
                                Translation = translation
                            };
                        }
                    }
                }
            }
        }

        public void SaveStringsToDatabase(IEnumerable<StringEntry> strings, bool overwrite)
        {
            foreach (var s in strings)
            {
                SaveStringToDatabase(s, overwrite);
            }
        }

        /// <summary>
        /// Saves to database. If culture is present, Translation entry is saved
        /// </summary>
        /// <param name="session"></param>
        /// <param name="input"></param>
        private void SaveStringToDatabase(StringEntry input, bool overwrite)
        {
            var translatableString = _localizableStringRepository.Table
                    .Where(s => s.StringKey == input.Key && s.Context == input.Context)
                    .FirstOrDefault();

            if (translatableString == null)
            {
                string path = input.Path;
                if (!path.Contains("{0}") && !string.IsNullOrEmpty(input.Culture))
                    path = path.Replace(input.Culture, "{0}");
                translatableString = new LocalizableStringRecord
                {
                    Path = path,
                    Context = input.Context,
                    StringKey = input.Key,
                    OriginalLanguageString = input.English
                };

                if (!translatableString.Path.Contains("{0}"))
                    throw new Exception("Path should contain {0}, but doesn't.\n" + translatableString.Path);

                _localizableStringRepository.Create(translatableString);
            }
            else if (translatableString.OriginalLanguageString != input.English)
            {
                translatableString.OriginalLanguageString = input.English;
                _localizableStringRepository.Update(translatableString);
            }

            if (!string.IsNullOrEmpty(input.Culture) && !string.IsNullOrEmpty(input.Translation))
            {
                var translation =
                    (from t in translatableString.Translations
                     where t.Culture.Equals(input.Culture)
                     select t).FirstOrDefault();

                if (translation == null)
                {
                    translation = new TranslationRecord
                    {
                        Culture = input.Culture,
                        Value = input.Translation
                    };
                    _translationRepository.Create(translation);

                    translatableString.AddTranslation(translation);
                }
                else if (overwrite)
                {
                    translation.Value = input.Translation;
                }
                _localizableStringRepository.Update(translatableString);

            }

            SetCacheInvalid();
        }

        public byte[] GetZipBytes(string culture)
        {
            var model = GetCultureDetailsViewModel(culture);

            if (model.Groups.Count == 0)
                return null;

            using (var stream = new MemoryStream())
            {
                using (var zip = new ZipOutputStream(stream))
                {
                    using (var writer = new StreamWriter(zip, Encoding.UTF8))
                    {
                        foreach (var translationGroup in model.Groups)
                        {
                            var file = new ZipEntry(translationGroup.Path) { DateTime = DateTime.Now };
                            zip.PutNextEntry(file);
                            writer.WriteLine(@"# Orchard resource strings - {0}
# Copyright (c) 2010 Outercurve Foundation
# All rights reserved
# This file is distributed under the BSD license
# This file is generated using the Coevery.Translations module
", culture);
                            foreach (var translation in translationGroup.Translations)
                            {
                                writer.WriteLine("#: " + ExportPoText(translation.Context));
                                writer.WriteLine("#| msgid \"" + ExportPoText(translation.Key) + "\"");
                                writer.WriteLine("msgctx \"" + ExportPoText(translation.Context) + "\"");
                                writer.WriteLine("msgid \"" + ExportPoText(translation.OriginalString) + "\"");
                                writer.WriteLine("msgstr \"" + ExportPoText(translation.LocalString) + "\"");
                                writer.WriteLine();
                            }
                            writer.Flush();
                        }
                    }
                    zip.IsStreamOwner = false;
                    zip.Close();
                }
                return stream.ToArray();
            }
        }

        public static string ExportPoText(string input)
        {
            if (input == null) return null;
            return input.Replace("\"", "\\\"");
        }

        public static string ImportPoText(string input)
        {
            return input.Replace("\\\"", "\"");
        }

        public void SavePoFilesToDisk()
        {
            foreach (var culture in GetTranslatedCultures())
                SavePoFilesToDisk(culture);
        }

        public void SavePoFilesToDisk(string culture)
        {
            var model = GetCultureDetailsViewModel(culture);

            if (model.Groups.Count == 0)
                return;

            foreach (var translationGroup in model.Groups)
            {
                string path = Path.Combine(_wca.GetContext().HttpContext.Server.MapPath("~"), translationGroup.Path);
                var file = new FileInfo(path);

                // delete the file if it already exists
                if (file.Exists)
                    file.Delete();

                // create directory if it doesn't exist
                else if (!file.Directory.Exists)
                    file.Directory.Create();

                using (var writer = File.CreateText(path))
                {
                    writer.WriteLine(@"# Orchard resource strings - {0}
# Copyright (c) 2010 Outercurve Foundation
# All rights reserved
# This file is distributed under the BSD license
# This file is generated using the Coevery.Translations module
", culture);
                    foreach (var translation in translationGroup.Translations)
                    {
                        writer.WriteLine("#: " + ExportPoText(translation.Context));
                        writer.WriteLine("#| msgid \"" + ExportPoText(translation.Key) + "\"");
                        writer.WriteLine("msgctx \"" + ExportPoText(translation.Context) + "\"");
                        writer.WriteLine("msgid \"" + ExportPoText(translation.OriginalString) + "\"");
                        writer.WriteLine("msgstr \"" + ExportPoText(translation.LocalString) + "\"");
                        writer.WriteLine();
                    }
                    writer.Flush();
                }
            }
        }

        public IEnumerable<string> GetTranslatedCultures()
        {
            return _cultureManager.ListCultures();
        }

        public CultureIndexViewModel GetCultures()
        {
            var model = new CultureIndexViewModel();
            var cultures = _translationRepository.Table
                    .ToList()
                    .GroupBy(t => t.Culture)
                    .Select(g => new
                    {
                        g.First().Culture,
                        Count = g.Count()
                    });

            cultures.ForEach(c =>
            {
                model.TranslationStates.Add(
                        c.Culture,
                        new CultureIndexViewModel.CultureTranslationState { NumberOfTranslatedStrings = c.Count });
            });

            model.NumberOfStringsInDefaultCulture = GetNumberOfTranslatableStrings();
            foreach (var cult in _cultureManager.ListCultures())
            {
                if (!model.TranslationStates.ContainsKey(cult))
                    model.TranslationStates.Add(cult, new CultureIndexViewModel.CultureTranslationState { NumberOfTranslatedStrings = 0 });
            }

            return model;
        }

        private int GetNumberOfTranslatableStrings()
        {
            //return (from t in session.Linq<LocalizableStringRecord>() select t).Count();
            return _localizableStringRepository.Table.Count();
        }

        public void UpdateTranslation(int id, string culture, string value)
        {
            var localizable = _localizableStringRepository.Get(id);
            var translation = localizable.Translations.Where(t => t.Culture == culture).FirstOrDefault();

            if (translation == null)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var newTranslation = new TranslationRecord
                    {
                        Culture = culture,
                        Value = value,
                        LocalizableStringRecord = localizable
                    };
                    localizable.Translations.Add(newTranslation);

                    _translationRepository.Create(newTranslation);
                    SetCacheInvalid();
                }
            }
            else if (String.IsNullOrEmpty(value))
            {
                _translationRepository.Delete(translation);
                SetCacheInvalid();
            }
            else if (String.Compare(translation.Value, value) != 0)
            {
                translation.Value = value;
                _translationRepository.Update(translation);
                SetCacheInvalid();
            }
        }

        public void RemoveTranslation(int id, string culture)
        {
            var translation = _localizableStringRepository.Get(id)
                .Translations.Where(t => t.Culture == culture).FirstOrDefault();

            if (translation != null)
            {
                translation.LocalizableStringRecord.Translations.Remove(translation);
                _translationRepository.Delete(translation);
                SetCacheInvalid();
            }
        }

        public IEnumerable<StringEntry> GetTranslationsFromZip(Stream stream)
        {
            var zip = new ZipInputStream(stream);
            ZipEntry zipEntry;
            while ((zipEntry = zip.GetNextEntry()) != null)
            {
                if (zipEntry.IsFile)
                {
                    var entrySize = (int)zipEntry.Size;
                    // Yeah yeah, but only a handful of people have upload rights here for the moment.
                    var entryBytes = new byte[entrySize];
                    zip.Read(entryBytes, 0, entrySize);
                    var content = entryBytes.ToStringUsingEncoding();
                    var cultureName = Path.GetFileName(Path.GetDirectoryName(zipEntry.Name));
                    var culture = cultureName;
                    foreach (var se in TranslateFile(zipEntry.Name, content, culture))
                        yield return se;
                }
            }
        }

        public bool IsCultureAllowed(string culture)
        {
            return true;

            // todo wtf?
            //var ctx = _wca.GetContext();
            //var rolesPart = ctx.CurrentUser.As<UserRolesPart>();
            //return (rolesPart != null && rolesPart.Roles.Contains(culture));
        }

        public IEnumerable<NotifyEntry> GetNotifications()
        {
            if (!IsCacheValid())
            {
                var request = _wca.GetContext().HttpContext.Request;
                UrlHelper urlHelper = new UrlHelper(request.RequestContext);
                var currentUrl = request.Url.PathAndQuery;

                yield return new NotifyEntry
                {
                    Message = T("Translation cache needs to be flushed. <a href=\"{0}\">Click here to flush!</a>", urlHelper.Action("FlushCache", "Admin", new { area = "Coevery.Translations", redirectUrl = currentUrl })),
                    Type = NotifyType.Warning
                };
            }
        }

        public void ResetCache()
        {
            _signals.Trigger("culturesChanged");
            _wca.GetContext().HttpContext.Application.Remove("coeveryTranslationsDirty");
        }

        private void SetCacheInvalid()
        {
            _wca.GetContext().HttpContext.Application["coeveryTranslationsDirty"] = true;
        }

        private bool IsCacheValid()
        {
            return !_wca.GetContext().HttpContext.Application.AllKeys.Contains("coeveryTranslationsDirty");
        }
    }
}