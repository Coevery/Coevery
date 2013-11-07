using System.Xml.Linq;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.MetaData.Services;

namespace Coevery.Entities.Services {
    public interface ISettingService : IDependency {
        SettingsDictionary ParseSetting(string setting);
        string CompileSetting(SettingsDictionary settings);
    }

    public class SettingService : ISettingService {
        private readonly ISettingsFormatter _settingsFormatter;

        public SettingService(
            ISettingsFormatter settingsFormatter) {
            _settingsFormatter = settingsFormatter;
        }

        public SettingsDictionary ParseSetting(string setting) {
            return string.IsNullOrWhiteSpace(setting)
                ? null
                : _settingsFormatter.Map(XElement.Parse(setting));
        }

        public string CompileSetting(SettingsDictionary settings) {
            return settings == null
                ? null
                : _settingsFormatter.Map(settings).ToString();
        }
    }
}