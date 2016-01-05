using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition.Hosting;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using UskyPlugsFrame.Interface;
using UskyPlugsFrame.BaseShow;
using Microsoft.Surface.Presentation.Controls;
using Decrypto;
using System.Net.NetworkInformation;
using System.Management;


namespace Soho.AppStart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region
        /// <summary>
        /// 对应的主窗体插件列表
        /// </summary>
        [ImportMany(typeof(IMainWindowPlugs))]
        public ObservableCollection<IMainWindowPlugs> MainWindowPlugsList { get; set; }

        /// <summary>
        /// 对应的子控件插件列表
        /// </summary>
        [ImportMany(typeof(IUserControlPlugs))]
        public ObservableCollection<IUserControlPlugs> UserControlPlugsList { get; set; }
        #endregion

        #region 捕获所有未指定的异常

        public App()
        {
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

        }

        #endregion

        /// <summary>
        /// MEF加载插件并返回主界面窗体
        /// </summary>
        /// <param name="mainWindowPlugsClassName">主界面窗体类名</param>
        /// <returns>主窗体界面</returns>
        private SurfaceWindow GetMainWindow(string mainWindowPlugsClassName)
        {
            try
            {
                var catalog = new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugs"));//加载这个文件夹下所有dll 同时给MainWindowPlugsList，UserControlPlugsList两个集合中加数据
                var container = new CompositionContainer(catalog);              //管理加载的进来插件            
                container.ComposeParts(this);                               //不知道干嘛的：从特性化对象的数组创建可组合部件，并在指定的组合容器中组合这些部件。
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions[0].Message);
            }
            foreach (SurfaceWindow ut in MainWindowPlugsList)               //集合中取要用的插件
            {
                if (ut.GetType().FullName.ToString().Equals(mainWindowPlugsClassName))  //列表中的插件名跟传过来名字匹配了
                {
                    return (SurfaceWindow)ut;                               //返回插件
                }
            }
            return null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CheckComputerInvadate()) return;
            string MainWindowClassName = System.Configuration.ConfigurationManager.AppSettings["MainWindow"];
            string[] ConfigKeys = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
            BaseMainWindow MainPage = (BaseMainWindow)GetMainWindow(MainWindowClassName);     //从配置文件中取出的主窗体名字来得到主窗体插件
            MainPage.WindowState = WindowState.Maximized;
            MainPage.WindowStyle = WindowStyle.None;
            MainPage.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainPage.UserControlList = UserControlPlugsList;
            foreach (string key in ConfigKeys)
            {
                if (key.Contains("UserControl_"))
                {
                    int id = Convert.ToInt32(key.Split('_')[1]);
                    string value = System.Configuration.ConfigurationManager.AppSettings[key];
                    MainPage.UserControlNameDic.Add(id, value);                                 //主界面把所有插件加进去
                }
                if (key.Contains("Config_"))
                {
                    string value = System.Configuration.ConfigurationManager.AppSettings[key];
                    MainPage.ConfigDic.Add(key, value);
                }
            }
            MainPage.Show();
        }


        /// <summary> 
        /// 获取Mac地址 
        /// </summary> 
        /// <returns></returns> 
        private string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length == 0) return string.Empty;
            return nics[0].GetPhysicalAddress().ToString();
        }
        public string getCPUID()
        {
            string cpuInfo = string.Empty;
            ManagementClass cimobject = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuInfo;
        }

        private bool CheckComputerInvadate()
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["UserCont_3"] == "99") return true;
                StringDecrypto EncryptionString = new StringDecrypto();
                string mac = EncryptionString.Decrypto(System.Configuration.ConfigurationManager.AppSettings["UserCont_1"]);
                string currentMac = GetMACAddress();
                string configCpu = EncryptionString.Decrypto(System.Configuration.ConfigurationManager.AppSettings["UserCont_2"]);
                string currentCPU = getCPUID();
                if (mac != currentMac && currentCPU != configCpu)
                {
                    return false;
                }
                string configTime = EncryptionString.Decrypto(System.Configuration.ConfigurationManager.AppSettings["UserCont_0"]);
                DateTime tempTime = DateTime.MinValue;
                if (!DateTime.TryParse(configTime, out tempTime))
                {
                    tempTime = DateTime.MinValue;
                }
                if (DateTime.Now > tempTime)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {

        }
    }
}
