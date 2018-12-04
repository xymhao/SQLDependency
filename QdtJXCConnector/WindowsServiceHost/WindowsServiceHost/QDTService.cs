using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace WindowsServiceHost
{
    public partial class QDTService : ServiceBase
    {
        public QDTService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var url = ConfigurationManager.AppSettings["WebApiUrl"];
                // Start OWIN host 
                WebApp.Start<Startup>(url: url);
                LogUtils.Error(string.Format(@"{0} 服务启动成功！", url));
            }
            catch (Exception e)
            {
                LogUtils.Error(e.Message);
                LogUtils.Error(e.StackTrace);
            }

        }

        protected override void OnStop()
        {
            var url = ConfigurationManager.AppSettings["WebApiUrl"];
            LogUtils.Error(string.Format(@"{0} 服务已停止！", url));
        }
    }
}
