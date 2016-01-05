using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UskyPlugsFrame.BaseShow;
using UskyPlugsFrame.Interface;
using System.Timers;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using SOHO.MainWindow.Model;
using SOHO.MainWindow.BLL;
using System.Reflection;
using System.Configuration;
using System.Threading;
using System.IO;


namespace SOHO.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            transitionName = GetTransitionName();
            LoadMenu();
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void LoadMenu()
        {
            ObservableCollection<DynamicImageButton> ButList = new ObservableCollection<DynamicImageButton>();
            List<MenuModel> Menulist = cb.GetMenuList();
            for (int i = 0; i < Menulist.Count; i++)
            {
                string ImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\" + Menulist[i].IconPath);
                DynamicImageButton dib = new DynamicImageButton();
                dib.IconImageUri = ImagePath;
                dib.Tag = Menulist[i].UserControlIndex;
                dib.Click += new RoutedEventHandler(dib_Click);
                ButList.Add(dib);
            }
            this.lv_Menu.ItemsSource = ButList;
            Checktime.Elapsed += new ElapsedEventHandler(Checktime_Elapsed);
            Checktime.Start();
        }

        void Checktime_Elapsed(object sender, ElapsedEventArgs e)
        {
            //////////////////////////////////////////////////////////////////////////
            //Edit by Ron.shen  修改为异步调用方法，避免界面无响应

            //////////////////////////////////////////////////////////////////////////
            this.Dispatcher.BeginInvoke(new Action(() =>
           {
               UpdateDataUI();
           }));
        }


        void dib_Click(object sender, RoutedEventArgs e)
        {
            DynamicImageButton btn_Dynamic = sender as DynamicImageButton;
            if (btn_Dynamic != null)
            {
                int index = (int)btn_Dynamic.Tag;
                base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(index));
            }
        }

        private static Configuration cfg;
        private TransitionName GetTransitionName()
        {
            int index = 0;
            Uri uri;
            Assembly assembly;
            ExeConfigurationFileMap map;

            map = new ExeConfigurationFileMap();
            assembly = Assembly.GetCallingAssembly();
            uri = new Uri(System.IO.Path.GetDirectoryName(assembly.CodeBase));
            map.ExeConfigFilename = System.IO.Path.Combine(uri.LocalPath, assembly.GetName().Name + ".exe.config");
            cfg = ConfigurationManager.OpenMappedExeConfiguration(map, 0);
            string transitionname = System.Configuration.ConfigurationManager.AppSettings["ShiftUI"];
            try
            {
                index = Int32.Parse(transitionname);
            }
            catch (Exception)
            {
                index = 0;
            }

            TransitionName tn = (TransitionName)index;
            return tn;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        string strReturnTime = "1";
        LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();

        ConfigModel configtime;
        #endregion

        System.Timers.Timer MainTime = null;
        System.Timers.Timer NotieTime = null;
        MainBLL cb = new MainBLL();
        System.Timers.Timer timer = new System.Timers.Timer(3000);
        System.Timers.Timer Checktime = new System.Timers.Timer(20 * 1000);
        private void BaseMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            configtime = cb.GetConfig();
            this.txt_time.Text = DateTime.Now.ToShortTimeString();
            this.txt_week.Text = "星期" + "日一二三四五六".Substring((int)System.DateTime.Now.DayOfWeek, 1);
            this.txt_date.Text = DateTime.Now.ToShortDateString();

            MainTime = new System.Timers.Timer(1000);
            MainTime.Elapsed += new ElapsedEventHandler(MainTime_Elapsed);
            MainTime.Start();

            NotieTime = new System.Timers.Timer(configtime.NoticeShowTime * 1000);
            NotieTime.Elapsed += new ElapsedEventHandler(NotieTime_Elapsed);

            base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(0));
        }

        void NotieTime_Elapsed(object sender, ElapsedEventArgs e)
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                Log(DateTime.Now + "图片被隐藏");
                this.NoticeUI.Visibility = Visibility.Hidden;
                this.NoticeUI.pup_Close.IsOpen = false;
                this.NoticeUI.CloseFlash();
               // this.NoticeUI.StopFlash();
                //this.AllowsTransparency = true;
                NotieTime.Stop();
                try
                {
                    SOHO.MainWindow.BLL.Keyboard.Press(Key.Space);
                }
                catch (System.Exception ex)
                {
                    Log(ex.Message);

                }
                
            }));
        }

        int it = 0;
        void MainTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.txt_time.Text = DateTime.Now.ToShortTimeString();
                this.txt_week.Text = "星期" + "日一二三四五六".Substring((int)System.DateTime.Now.DayOfWeek, 1);
                this.txt_date.Text = DateTime.Now.ToString("yyyy/MM/dd");
                LastInputInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastInputInfo);
                GetLastInputInfo(ref LastInputInfo);
                uint i = ((uint)Environment.TickCount - LastInputInfo.dwTime);
                int t = int.Parse((i / 1000).ToString());
                if (t >= (int.Parse(strReturnTime) * configtime.IdleTime * 1) && t % (int.Parse(strReturnTime) * configtime.IdleTime * 1) == 0)
                {
                    if (this.CurrentUserControlName.Equals(GetUserControlNameFromDic(0)))
                    {
                        //this.AllowsTransparency = false;
                       
                        it++;
                        this.NoticeUI.btn_Close.Visibility = Visibility.Collapsed;
                        this.NoticeUI.Visibility = Visibility.Visible;
                        Log(DateTime.Now + "ChoseNoticeType函数被调用"+it+"次");
                        this.NoticeUI.ChoseNoticeType();
                        SOHO.MainWindow.BLL.Keyboard.Press(Key.Space);
                        NotieTime.Start();
                    }
                }
            }));

        }

        private void Log(string str)
        {
            using (StreamWriter  fs=new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"log.txt"),true))
            {
                fs.WriteLine(str);
            }
        }

        #region 获取消息更新数据
        private void UpdateDataUI()
        {
            int flag = cb.GetUpdateMessage();
            if (flag == 1)
            {
                //MainTime.Stop();
                base.UpdateData();
                configtime = cb.GetConfig();
                cb.SetUpdateMessage();
                //MainTime.Start();
            }
        }

        #endregion

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //LoopPanelVertical.StartTimer();
            timer.Stop();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(0));


        }

        private void Notice_Click(object sender, RoutedEventArgs e)
        {
            base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(1));
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Floor_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Overview_Click(object sender, RoutedEventArgs e)
        {
            base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(4));
        }

        private void Service_Click(object sender, RoutedEventArgs e)
        {
            base.NavigationToUserPage(this.Grid_Container, GetUserControlNameFromDic(5));
        }
    }
}
