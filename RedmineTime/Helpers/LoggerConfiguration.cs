using System.Configuration;
using System.Linq;

namespace Unosquare.RedmineTime.Helpers
{
    public class LoggerConfiguration : ILoggerConfiguration
    {
        public string RedmineApiKey
        {
            get
            {
                return LiteDecrypt(Properties.Settings.Default.RedmineApiKey);
            }
            set
            {
                Properties.Settings.Default.RedmineApiKey = LiteEncrypt(value);
                Properties.Settings.Default.Save();
            }
        }

        public string RedmineUrl
        {
            get
            {
                return Properties.Settings.Default.RedmineServiceUrl;
            }
            set
            {
                Properties.Settings.Default.RedmineServiceUrl = value;
                Properties.Settings.Default.Save();
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

    }
}