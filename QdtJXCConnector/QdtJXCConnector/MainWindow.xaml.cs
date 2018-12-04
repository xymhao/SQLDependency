using System;
using System.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using Microsoft.Win32;
using QdtJXCConnector.WCFListenService;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.ServiceProcess;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Utils;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.Windows.Media.Imaging;
using Utility;

namespace QdtJXCConnector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        string serviceName = ConfigurationManager.AppSettings["ServiceName"];

        public MainWindow()
        {
            try
            {                
                log4net.Config.XmlConfigurator.Configure();
                base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ServiceUtility.InstallWebApiService();
                InitializeComponent();
                InitNotifyIconMenu();
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void InitNotifyIconMenu()
        {
            //最小化菜单
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "签到通同步软件运行中... ...";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "签到通同步软件运行中... ...";
            string path = AppDomain.CurrentDomain.BaseDirectory;
            this.notifyIcon.Icon = new System.Drawing.Icon(path + "favicon.ico");
            this.notifyIcon.Visible = true;
            //窗体图标
            Uri iconUri = new Uri(path + "favicon@2.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            this.Title = "签到通中间件";
            //打开菜单项
            MenuItem open = new MenuItem("打开配置", Open_Click);
            //退出菜单项
            MenuItem exit = new MenuItem("退出", Exit_Click);
            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);
            //窗体最小化
            this.ResizeMode = ResizeMode.CanMinimize;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Open_Click(o, e);
            });
        }

        //退出程序
        private void Exit_Click(object sender, EventArgs e)
        {
            try
            {
                string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
                Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);
                AppSettingsSection appSettings = config.GetSection("appSettings") as AppSettingsSection;
                appSettings.Settings.Remove("IsStartWebApi");
                appSettings.Settings.Add("IsStartWebApi", "0");
                config.Save();
                BaseSettingService.GetInstance().Dispose();
                ServiceUtility.StopSercice();
                //UnInstallmyService();
                notifyIcon.Dispose();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
                Environment.Exit(1);
            }
        }

        public string GetWindowsServiceInstallPath()
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            path = path.Replace("\"", string.Empty);//替换掉双引号
            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        public static void UnInstallWebApiService()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Service" + "\\WindowsServiceHost.exe";
            AssemblyInstaller AssemblyInstaller1 = new AssemblyInstaller();
            AssemblyInstaller1.UseNewContext = true;
            AssemblyInstaller1.Path = filepath;
            AssemblyInstaller1.Uninstall(null);
            AssemblyInstaller1.Dispose();
        }

        //打开窗体
        private void Open_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        //窗体托盘化
        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        //窗体加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //选项初始化
                tb_ServerAddress.Text = ConfigurationManager.AppSettings["WebApiUrl"];
                tb_SQLServerAddress.Text = ConfigurationManager.AppSettings["DataSource"];
                var decryptPWT = ConfigurationManager.AppSettings["Password"];
                if(!string.IsNullOrEmpty(decryptPWT))
                    decryptPWT= EncryptUtility.MD5Decrypt(decryptPWT);
                tb_SQLPassword.Password = decryptPWT;
                tb_SQLUserName.Text = ConfigurationManager.AppSettings["AdminID"];

                if (ConfigurationManager.AppSettings["runningWithStart"].ToString() == "0")
                {
                    cb_AutoStar.IsChecked = false;
                }
                else
                {
                    cb_AutoStar.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
            }
        }

        private void StartWebApi()
        {
            //判断是否连接数据库成功
            DataBaseUtility.Validate();
            /*Web服务开起*/
            string baseAddress = ConfigurationManager.AppSettings["WebApiUrl"];
            UpdateWebApiServiceUrl();
            ServiceUtility.StartSercice();
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            ApiServiceTest();
        }

        private void VerifyPort(int port)
        {
            int portNumber = Convert.ToInt32(port);
            if (portNumber < 0 || portNumber > 65535)
            {
                System.Windows.Forms.MessageBox.Show("端口号已被占用，请更换端口号。");
                return;
            }

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    throw new Exception("端口号已被占用，请更换端口号。");
                }
            }
        }

        private void ApiServiceTest()
        {
            string baseAddress = ConfigurationManager.AppSettings["WebApiUrl"];
            HttpClient client = new HttpClient();
            var response = client.GetAsync(baseAddress.TrimEnd('/').TrimEnd('\\') + "/api/BaseValues/GetVersion").Result;
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new Exception("服务启动失败，请检查中间件服务器地址。");
            }
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_ServerAddress.Text) ||
               string.IsNullOrWhiteSpace(tb_SQLServerAddress.Text) ||
               string.IsNullOrWhiteSpace(tb_SQLPassword.Password) ||
               string.IsNullOrWhiteSpace(tb_SQLUserName.Text))
            {
                System.Windows.Forms.MessageBox.Show("配置项不能为空");
                return;
            }
            //处理字符大小写输入错误
            try
            {
                HandleErrorChars();
                ServiceUtility.StopSercice(true);
                VirifyMiddleWareAddress();
                //VirifySQLAddress();
            }
            catch (Exception se)
            {
                LogUtils.Error(se.Message);
                LogUtils.Error(se.StackTrace);
                System.Windows.Forms.MessageBox.Show("错误:" + se.Message);
                return;
            }
            try
            {
                //读取程序集的配置文件
                string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
                Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);
                //获取appSettings节点
                AppSettingsSection appSettings = config.GetSection("appSettings") as AppSettingsSection;
                //删除name，然后添加新值
                appSettings.Settings.Remove("WebApiUrl");
                appSettings.Settings.Add("WebApiUrl", tb_ServerAddress.Text);

                appSettings.Settings.Remove("DataSource");
                appSettings.Settings.Add("DataSource", tb_SQLServerAddress.Text);
                appSettings.Settings.Remove("Password");
                var password = EncryptUtility.MD5Encrypt(tb_SQLPassword.Password);
                appSettings.Settings.Add("Password", password);
                appSettings.Settings.Remove("AdminID");
                appSettings.Settings.Add("AdminID", tb_SQLUserName.Text);
                appSettings.Settings.Remove("FirstUser");
                appSettings.Settings.Add("FirstUser", "0");
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                StartWebApi();
                //监听服务运行
                BaseSettingService.GetInstance().Start();
                System.Windows.Forms.MessageBox.Show("同步服务启动成功");
                ChangeControlStatus(0);
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
                System.Windows.Forms.MessageBox.Show("错误:"+ex.Message);
            }
        }

        public void VirifySQLAddress()
        {
            string ipRegexSte = @"((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))";

            var addressArr = tb_SQLServerAddress.Text.Replace("http:", "").Split(',');
            var url = addressArr[0].Trim('\\').Trim('/');
            if (!Regex.IsMatch(url, ipRegexSte))
            {
                throw new Exception(@"SQL服务器地址错误,格式如127.0.0.1,1433");
            }
        }

        public void VirifyMiddleWareAddress()
        {
            string ipRegexSte = @"(http|https)://((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))";

            //非域名情况
            if (rb_IP.IsChecked.Equals(true))
            {
                var addressArr = tb_ServerAddress.Text.Replace("http:", "").Split(':');
                var url = addressArr[0].Trim('\\').Trim('/');
                var port = addressArr.Length > 1 ? addressArr[1].Trim('\\').Trim('/') : "";

                if (!addressArr.Length.Equals(2) || !Regex.IsMatch(tb_ServerAddress.Text, ipRegexSte))
                {
                    throw new Exception(@"服务器地址格式错误，格式如http://127.0.0.1:8000/");
                }

                if (!string.IsNullOrWhiteSpace(port))
                {
                    int portNumber = Convert.ToInt32(port.Trim('\\').Trim('/'));
                    VerifyPort(Convert.ToInt32(portNumber));
                }
            }
        }

        public void HandleErrorChars()
        {
            tb_ServerAddress.Text = tb_ServerAddress.Text.Replace("：", ":");
            tb_ServerAddress.Text = tb_ServerAddress.Text.Replace("。", ".");
            tb_ServerAddress.Text = tb_ServerAddress.Text.Replace('\\', '/');

            tb_SQLServerAddress.Text = tb_SQLServerAddress.Text.Replace('\\', '/');
            tb_SQLServerAddress.Text = tb_SQLServerAddress.Text.Replace("，", ",");
            tb_SQLServerAddress.Text = tb_SQLServerAddress.Text.Replace("。", ".");
        }

        public void ChangeControlStatus(int status)
        {
            if (status.Equals(0))
            {
                tb_ServerAddress.IsReadOnly = true;
                tb_SQLServerAddress.IsReadOnly = true;
                tb_SQLUserName.IsReadOnly = true;
                tb_SQLPassword.IsEnabled = false;
                rb_Domain.IsEnabled = false;
                rb_IP.IsEnabled = false;
                btn_OK.Visibility = Visibility.Hidden;
                btn_Stop.Visibility = Visibility.Visible;
            }
            else
            {
                tb_ServerAddress.IsReadOnly = false;
                tb_SQLServerAddress.IsReadOnly = false;
                tb_SQLUserName.IsReadOnly = false;
                tb_SQLPassword.IsEnabled = true;
                rb_Domain.IsEnabled = true;
                rb_IP.IsEnabled = true;
                btn_OK.Visibility = Visibility.Visible;
                btn_Stop.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_window_close_Click(object sender, RoutedEventArgs e)
        {
            //隐藏窗体
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void btn_window_small_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; //设置窗口最小化
        }

        private void WrapPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等  
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 是否开机启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsAdministrator())
                {
                    cb_AutoStar.IsChecked = false;
                    System.Windows.Forms.MessageBox.Show("错误:开机启动需要管理员权限，请以管理员身份运行程序");
                    return;
                }
                RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (rgkRun == null)
                {
                    rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
                Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);
                //获取appSettings节点
                AppSettingsSection appSettings = config.GetSection("appSettings") as AppSettingsSection;
                string path = Process.GetCurrentProcess().MainModule.FileName;
                if ((bool)cb_AutoStar.IsChecked)
                {
                    /*开机启动*/
                    rgkRun.SetValue("QdtJXCConnector", path);
                    appSettings.Settings.Remove("runningWithStart");
                    appSettings.Settings.Add("runningWithStart", "1");
                    cb_AutoStar.IsChecked = true;
                }
                else
                {
                    rgkRun.DeleteValue("QdtJXCConnector");
                    appSettings.Settings.Remove("runningWithStart");
                    appSettings.Settings.Add("runningWithStart", "0");
                    cb_AutoStar.IsChecked = false;
                }
                config.Save();
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
                System.Windows.Forms.MessageBox.Show("错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            ServiceUtility.StopSercice();
            BaseSettingService.GetInstance().Dispose();
            ChangeControlStatus(1);
            System.Windows.Forms.MessageBox.Show("服务已停止");
        }

        //修改服务配置项
        public static void UpdateWebApiServiceUrl()
        {
            var url = ConfigurationManager.AppSettings["WebApiUrl"];
            string dataSource = ConfigurationManager.AppSettings["DataSource"];
            string userID = ConfigurationManager.AppSettings["AdminID"];
            string password = ConfigurationManager.AppSettings["Password"];
            string CurrentExeDirectory = AppDomain.CurrentDomain.BaseDirectory + "Service" + "\\WindowsServiceHost.exe";            
            Configuration config = ConfigurationManager.OpenExeConfiguration(CurrentExeDirectory);
            //获取appSettings节点
            AppSettingsSection appSettings = config.GetSection("appSettings") as AppSettingsSection;
            appSettings.Settings.Remove("WebApiUrl");
            appSettings.Settings.Add("WebApiUrl", url);

            appSettings.Settings.Remove("DataSource");
            appSettings.Settings.Add("DataSource", dataSource);
            appSettings.Settings.Remove("AdminID");
            appSettings.Settings.Add("AdminID", userID);
            appSettings.Settings.Remove("Password");
            appSettings.Settings.Add("Password", password);

            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        //本地webApi服务测试
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string baseAddress = ConfigurationManager.AppSettings["WebApiUrl"];
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress.TrimEnd('/').TrimEnd('\\') + "/api/BaseValues/GetVersion").Result;
                System.Windows.MessageBox.Show(response + response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
                LogUtils.Error(ex.StackTrace);
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

    }
}
