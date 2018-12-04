using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHost
{
    public static class AppConfigSetting
    {
        public static void Setting(string webApiUrl)
        {
            var url = ConfigurationManager.AppSettings["WebApiUrl"];
            //读取程序集的配置文件
            string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);
            //获取appSettings节点
            AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");
            appSettings.Settings.Remove("WebApiUrl");
            appSettings.Settings.Add("WebApiUrl", webApiUrl);
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
