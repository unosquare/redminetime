using System.Configuration;
using System.Linq;

namespace RedmineLogger.Helpers
{
    public class LoggerConfiguration : ILoggerConfiguration
    {
        //private const string RedmineApiKeySetting = "RedmineApiKey";
        //private const string RedmineUrlSetting = "RedmineUrl";

        public string RedmineApiKey
        {
            get
            {
                return LiteDecrypt(Properties.Settings.Default.RedmineApiKey);
                //return LiteDecrypt(ConfigurationManager.AppSettings[RedmineApiKeySetting]);
            }
            set
            {
                Properties.Settings.Default.RedmineApiKey = LiteEncrypt(value);
                Properties.Settings.Default.Save();
                //ApplySettings(RedmineApiKeySetting, LiteEncrypt(value));
            }
        }

        public string RedmineUrl
        {
            get
            {
                return Properties.Settings.Default.RedmineServiceUrl;
                // ConfigurationManager.AppSettings[RedmineUrlSetting];
            }
            set
            {
                Properties.Settings.Default.RedmineServiceUrl = value;
                Properties.Settings.Default.Save();
                //ApplySettings(RedmineUrlSetting, value);
            }
        }

        private static string LiteDecrypt(string value)
        {
            // TODO: Perhaps make a more secure encription. It is enough for nw.
            return value == null ? null : new string(value.Reverse().ToArray());
        }

        private static string LiteEncrypt(string value)
        {
            // TODO: Perhaps make a more secure encription. It is enough for nw.
            return value == null ? null : new string(value.Reverse().ToArray());
        }

        //private void ApplySettings(string key, string value)
        //{
        //    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    var settings = configFile.AppSettings.Settings;
        //    if (settings[key] == null)
        //    {
        //        settings.Add(key, value);
        //    }
        //    else
        //    {
        //        settings[key].Value = value;
        //    }
        //    configFile.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        //}
    }
}