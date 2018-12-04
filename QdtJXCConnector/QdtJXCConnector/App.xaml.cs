using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace QdtJXCConnector
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            bool createNew;
            try
            {
                int.TryParse(ConfigurationManager.AppSettings["AllowYun"], out int allow);
                if (0.Equals(allow))
                {
                    string targetExeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    string productName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
                    //判断是否已启动。
                    using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, productName, out createNew))
                    {
                        if (createNew)
                        {
                            //MainWindow.xaml
                            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                            Run();
                        }
                        else
                        {
                            Environment.Exit(1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.Error(e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }
    }
}
