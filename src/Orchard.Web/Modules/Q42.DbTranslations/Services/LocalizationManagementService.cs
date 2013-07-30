using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fluent.Zip;
using Orchard;
using Q42.DbTranslations.Models;
using Path = Fluent.IO.Path;
using Orchard.Logging;

namespace Q42.DbTranslations.Services
{
  public interface ILocalizationManagementService : IDependency
  {
    void InstallTranslation(byte[] zippedTranslation, string sitePath);
    byte[] PackageTranslations(string cultureCode, string sitePath);
    IEnumerable<StringEntry> ExtractDefaultTranslation(string sitePath);
    void SyncTranslation(string sitePath, string cultureCode);
  }

  public class LocalizationManagementService : ILocalizationManagementService
  {

    private ILogger Logger;
    public LocalizationManagementService()
    {
      Logger = NullLogger.Instance;
    }

    private static readonly Regex ResourceStringExpression =
        new Regex(
            @"T\(((@"".*"")|(""([^""\\]|\\.)*?""))([^)""]*)\)",
            RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex PluralStringExpression =
        new Regex(
            @"T.Plural\(((@"".*"")|(""([^""\\]|\\.)*?""))([^)""]*),\s*((@"".*"")|(""([^""\\]|\\.)*?""))([^)""]*)\)",
            RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex NamespaceExpression =
        new Regex(
            @"namespace ([^\s]*)\s*{",
            RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex ClassExpression =
        new Regex(
            @"class\s+([^\s:{]+)",
            RegexOptions.Multiline | RegexOptions.Compiled);

    public void InstallTranslation(byte[] zippedTranslation, string sitePath)
    {
      var siteRoot = Path.Get(sitePath);
      ZipExtensions.Unzip(zippedTranslation,
          (path, contents) =>
          {
            if (path.Extension == ".po")
            {
              var tokens = path.Tokens;
              var destPath = siteRoot.Combine(tokens);
              // If a translation file is for a module or a theme, only install it
              // if said module or theme exists already. Otherwise, skip.
              if ((!tokens[0].Equals("modules", StringComparison.OrdinalIgnoreCase) &&
                   !tokens[0].Equals("themes", StringComparison.OrdinalIgnoreCase)) ||
                    siteRoot.Combine(tokens[0], tokens[1]).IsDirectory)
              {
                destPath.Write(contents);
              }
            }
          });
    }

    public byte[] PackageTranslations(string cultureCode, string sitePath)
    {
      var site = Path.Get(sitePath);
      var translationFiles = site
          .Files("orchard.*.po", true)
          .Where(p => p.Parent().FileName.Equals(cultureCode, StringComparison.OrdinalIgnoreCase))
          .MakeRelativeTo(site);
      return ZipExtensions.Zip(
          translationFiles,
          p => site.Combine(p).ReadBytes());
    }

    public IEnumerable<StringEntry> ExtractDefaultTranslation(string sitePath)
    {
      var result = new List<StringEntry>();
      var site = Path.Get(sitePath);
      string corePoPath = System.IO.Path.Combine(
          "Core", "App_Data",
          "Localization", "{0}",
          "orchard.core.po");
      var rootPoPath = System.IO.Path.Combine(
          "App_Data", "Localization", "{0}",
          "orchard.root.po");

      // Extract resources for module manifests
      site.Files("module.txt", true)
          .Read((content, path) =>
          {
            var moduleName = path.Parent().FileName;
            var poPath =
                path.MakeRelativeTo(sitePath).Tokens[0].Equals("core", StringComparison.OrdinalIgnoreCase) ?
                     corePoPath : GetModuleLocalizationPath(site, moduleName);
            result.AddRange(ExtractPoFromManifest(poPath, content, path, site));
          });
      // Extract resources for theme manifests
      site.Files("theme.txt", true)
          .Read((content, path) =>
          {
            var themeName = path.Parent().FileName;
            var poPath = GetThemeLocalizationPath(site, themeName);
            result.AddRange(ExtractPoFromManifest(poPath, content, path, site));
          });
      // Extract resources from views and cs files, for the web site
      // as well as for the framework and Azure projects.
      if (site.Parent().Combine("Orchard").Exists)
        site = site.Add(site.Parent().Combine("Orchard"));
      if (site.Parent().Combine("Orchard.Azure").Exists)
        site = site.Add(site.Parent().Combine("Orchard.Azure"));

      site.ForEach(p =>
              p.Files("*", true)
              .WhereExtensionIs(".cshtml", ".aspx", ".ascx", ".cs")
              .Grep(
                  ResourceStringExpression,
                  (path, match, contents) =>
                  {
                    var str = Unescape(match.Groups[1].ToString()).Trim('"');
                    result.AddRange(DispatchResourceString(corePoPath, rootPoPath, site, path, p, contents, str));
                  }
              )
              .Grep(
                  PluralStringExpression,
                  (path, match, contents) =>
                  {
                    var str = Unescape(match.Groups[1].ToString()).Trim('"');
                    result.AddRange( DispatchResourceString(corePoPath, rootPoPath, site, path, p, contents, str));
                    str = Unescape(match.Groups[6].ToString()).Trim('"');
                    result.AddRange( DispatchResourceString(corePoPath, rootPoPath, site, path, p, contents, str));
                  }
              ));
      return result;
    }

    public void SyncTranslation(string sitePath, string cultureCode)
    {
      Path.Get(sitePath)
          .Files("orchard.*.po", true)
          .Where(p => p.Parent().FileName.Equals("en-US", StringComparison.OrdinalIgnoreCase))
          .ForEach(baselinePath =>
          {
            var path = baselinePath.Parent().Parent().Combine(cultureCode, baselinePath.FileName);
            var translations = new List<StringEntry>();
            if (path.Exists)
            {
              path.Open(inStream => ReadTranslations(inStream, translations), FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            path.Parent().CreateDirectory();
            path.Open(outStream =>
            {
              var englishTranslations = new List<StringEntry>();
              baselinePath.Open(baselineStream => ReadTranslations(baselineStream, englishTranslations), FileMode.Open, FileAccess.Read, FileShare.Read);
              using (var writer = new StreamWriter(outStream))
              {
                foreach (var englishTranslation in englishTranslations)
                {
                  var entry = englishTranslation;
                  var translation = translations.Where(
                      t => t.Context == entry.Context &&
                           t.Key == entry.Key).FirstOrDefault();
                  if (translation == default(StringEntry) ||
                      translation.Translation == null ||
                      translation.Translation.Equals(@"msgstr """""))
                  {

                    writer.WriteLine("# Untranslated string");
                  }
                  writer.WriteLine(entry.Context);
                  writer.WriteLine(entry.Key);
                  writer.WriteLine(entry.English);
                  if (translation != null)
                  {
                    translation.Used = true;
                    writer.WriteLine(translation.Translation);
                  }
                  else
                  {
                    writer.WriteLine("msgstr \"\"");
                  }
                  writer.WriteLine();
                }
                foreach (var translation in translations.Where(t => !t.Used))
                {
                  writer.WriteLine("# Obsolete translation");
                  writer.WriteLine(translation.Context);
                  writer.WriteLine(translation.Key);
                  writer.WriteLine(translation.English);
                  writer.WriteLine(translation.Translation);
                  writer.WriteLine();
                }
              }
            }, FileMode.Create, FileAccess.Write, FileShare.None);
          });
    }

    private static void ReadTranslations(FileStream inStream, List<StringEntry> translations)
    {
      var translation = new StringEntry();
      var comparer = new StringEntryEqualityComparer();
      using (var reader = new StreamReader(inStream))
      {
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();
          if (line != null)
          {
            if (line.StartsWith("#: "))
            {
              translation.Context = line;
            }
            else if (line.StartsWith("#| msgid "))
            {
              translation.Key = line;
            }
            else if (line.StartsWith("msgid "))
            {
              translation.English = line;
            }
            else if (line.StartsWith("msgstr "))
            {
              translation.Translation = line;
              if (!translations.Contains(translation, comparer))
              {
                translations.Add(translation);
              }
              translation = new StringEntry();
            }
          }
        }
      }
    }

    private IEnumerable<StringEntry> DispatchResourceString(
        string corePoPath,
        string rootPoPath,
        Path sitePath,
        Path path,
        Path currentInputPath,
        string contents, 
        string str)
    {
      var current = "~/" + path.MakeRelativeTo(currentInputPath).ToString().Replace('\\', '/');
      
      // exclude items from the /obj/ directories, this is where packages reside
      if (!path.FullPath.Contains("\\obj\\"))
      {
        var context = current;
        if (path.Extension == ".cs")
        {
          var ns = NamespaceExpression.Match(contents).Groups[1].ToString();
          var type = ClassExpression.Match(contents).Groups[1].ToString();
          context = ns + "." + type;
        }
        string targetPath = null;
        if (current.StartsWith("~/core/", StringComparison.OrdinalIgnoreCase))
        {
          targetPath = corePoPath;
        }
        else if (current.StartsWith("~/themes/", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            targetPath = GetThemeLocalizationPath(sitePath, current.Substring(9, current.IndexOf('/', 9) - 9)).ToString();
          }
          catch (ArgumentOutOfRangeException ex)
          {
            Logger.Error(ex, "Error substinging {0}, skipping string {1}!", current, str);
          }
        }
        else if (current.StartsWith("~/modules/", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            targetPath = GetModuleLocalizationPath(sitePath, current.Substring(10, current.IndexOf('/', 10) - 10)).ToString();
          }
          catch (ArgumentOutOfRangeException ex)
          {
            Logger.Error(ex, "Error substinging {0}, skipping string {1}!", current, str);
          }
        }
        else if (current.StartsWith("~/obj/", StringComparison.OrdinalIgnoreCase))
        {
          targetPath = null;
        }
        else
        {
          targetPath = rootPoPath;
        }

        if (!string.IsNullOrEmpty(targetPath))
        {
          yield return new StringEntry
          {
            Culture = null,
            Context = context,
            Key = str,
            English = str,
            Translation = str,
            Path = targetPath.Replace('\\', '/')
          };
        }
      }
    }

    private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

    private static string Unescape(string str)
    {
      StringBuilder sb = null;
      bool escaped = false;
      for (var i = 0; i < str.Length; i++)
      {
        var c = str[i];
        if (escaped)
        {
          if (sb == null)
          {
            sb = new StringBuilder(str.Length);
            if (i > 1)
            {
              sb.Append(str.Substring(0, i - 1));
            }
          }
          char unescaped;
          if (_escapeTranslations.TryGetValue(c, out unescaped))
          {
            sb.Append(unescaped);
          }
          else
          {
            // General rule: \x ==> x
            sb.Append(c);
          }
          escaped = false;
        }
        else
        {
          if (c == '\\')
          {
            escaped = true;
          }
          else if (sb != null)
          {
            sb.Append(c);
          }
        }
      }
      return sb == null ? str : sb.ToString();
    }

    private static readonly Regex FeatureNameExpression = new Regex(@"^\s+([^\s:]+):\s*$");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="poPath">PO file that the translation would be in traditional po setup</param>
    /// <param name="manifest">content of the manifest file</param>
    /// <param name="manifestPath">path to the manifest file</param>
    /// <param name="rootPath">path to the root of the site</param>
    /// <returns></returns>
    private static IEnumerable<StringEntry> ExtractPoFromManifest(
        string poPath,
        string manifest,
        Path manifestPath,
        Path rootPath)
    {
      if (!manifestPath.FullPath.Contains("\\obj\\"))
      {
        var context = "~/" + manifestPath.MakeRelativeTo(rootPath).ToString()
                                 .Replace('\\', '/');
        var reader = new StringReader(manifest);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          var split = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)
              .Select(s => s.Trim()).ToArray();
          if (split.Length == 2)
          {
            var key = split[0];
            if (new[] { "Name", "Description", "Author", "Website", "Tags" }.Contains(key))
            {
              var value = split[1];
              yield return new StringEntry
              {
                Culture = null,
                Context = context,
                Key = key,
                English = value,
                Translation = value,
                Path = poPath.Replace('\\', '/')
              };
            }
          }
          if (line.StartsWith("Features:"))
          {
            var feature = "";
            while ((line = reader.ReadLine()) != null)
            {
              var match = FeatureNameExpression.Match(line);
              if (match.Success)
              {
                feature = match.Groups[1].Value;
                continue;
              }
              split = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => s.Trim()).ToArray();
              if (split.Length != 2) continue;
              var key = split[0];
              if (new[] { "Name", "Description", "Category" }.Contains(key))
              {
                var value = split[1];
                yield return new StringEntry
                {
                  Culture = null,
                  Context = context,
                  Key = feature + "." + key,
                  English = value,
                  Translation = value,
                  Path = poPath.Replace('\\', '/')
                };
              }
            }
          }
        }
      }
    }

    private static string GetThemeLocalizationPath(Path siteRoot, string themeName)
    {
      return System.IO.Path.Combine(
          "Themes", themeName, "App_Data",
          "Localization", "{0}",
          "orchard.theme.po");
    }

    private static string GetModuleLocalizationPath(Path siteRoot, string moduleName)
    {
      return System.IO.Path.Combine(
          "Modules", moduleName, "App_Data",
          "Localization", "{0}",
          "orchard.module.po");
    }

  }
}