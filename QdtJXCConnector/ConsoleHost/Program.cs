using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            string baseAddress = ConfigurationManager.AppSettings["WebApiUrl"];
            // Start OWIN host 
            WebApp.Start<Startup>(url: baseAddress);
            Console.WriteLine("已启动地址：" + baseAddress);
            LogUtils.Error(string.Format(@"服务启动成功！"));

            Console.ReadLine();
        }
    }
}
