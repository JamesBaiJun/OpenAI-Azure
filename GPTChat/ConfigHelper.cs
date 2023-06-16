using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPTChat
{
    public class ConfigHelper
    {
        public static string GetConfig(string key)
        {

            var appSettings = ConfigurationManager.AppSettings[key];
            return appSettings.ToString();
        }

        public static void SaveConfig(string key, string val)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[key].Value = val;
            cfa.AppSettings.SectionInformation.ForceSave = true;
            cfa.Save(ConfigurationSaveMode.Modified);
        }
    }
}
