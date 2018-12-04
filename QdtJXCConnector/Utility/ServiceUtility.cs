using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Utility
{
    public class ServiceUtility
    {
        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsExistService()
        {
            bool status = false;
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            string firstUse = ConfigurationManager.AppSettings["FirstUser"];

            ServiceController[] scServices = ServiceController.GetServices();
            foreach (var s in scServices)
            {
                if (s.ServiceName.Equals(serviceName))
                {
                    if (s.CanStop)
                    {
                        s.Stop();
                    }
                    s.Refresh();
                    var folder = AppDomain.CurrentDomain.BaseDirectory + "Service\\WindowsServiceHost.exe";
                    ChangeRegistryPath(serviceName, folder);//如果和当前路径不同，删除Service
                    return true;
                }
            }
            return status;
        }

        public static void ChangeRegistryPath(string serviceName, string curruntPath)
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            var regist = Registry.LocalMachine.OpenSubKey(key, true);
            if (regist == null)
                return;
            string path = regist.GetValue("ImagePath").ToString();
            curruntPath = string.Format("\"{0}\"", curruntPath);
            if (!path.Equals(curruntPath))
            {
                regist.SetValue("ImagePath", curruntPath);
            }
            path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        public static void InstallWebApiService()
        {
            if (ServiceUtility.IsExistService())
                return;
            IDictionary mySavedState = new Hashtable();
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Service\\WindowsServiceHost.exe";

            AssemblyInstaller AssemblyInstaller = new AssemblyInstaller();
            AssemblyInstaller.UseNewContext = true;
            AssemblyInstaller.Path = filepath;
            AssemblyInstaller.Install(mySavedState);
            AssemblyInstaller.Commit(mySavedState);
            AssemblyInstaller.Dispose();
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        public static void UnInstallWebApiService()
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "Service\\WindowsServiceHost.exe";
                AssemblyInstaller AssemblyInstaller1 = new AssemblyInstaller();
                AssemblyInstaller1.UseNewContext = true;
                AssemblyInstaller1.Path = filepath;
                AssemblyInstaller1.Uninstall(null);
                AssemblyInstaller1.Dispose();
            }
            catch (Exception e)
            {
                LogUtils.Error(e.Message);
                LogUtils.Error(e.StackTrace);
            }

        }

        public static void StartSercice()
        {
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.CanStop)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
            }
            else
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
        }

        public static void StopSercice(bool isWait = false)
        {
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.CanStop)
            {
                serviceController.Stop();
            }
            if (isWait)
            {
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }
    }
}
