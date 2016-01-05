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
using SOHO.MainWindow.Model;
using SOHO.MainWindow.BLL;
using System.Timers;
using System.IO;

namespace SOHO.MainWindow
{
    /// <summary>
    /// Interaction logic for NotifyControl.xaml
    /// </summary>
    public partial class NotifyControl : UserControl
    {
        public NotifyControl()
        {
            InitializeComponent();
        }
        private void btn_Close_TouchDown(object sender, TouchEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this.pup_Close.IsOpen = false;
        }

        MainBLL Mb = new MainBLL();
        bool FullScreen = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void StopFlash()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
           {
               this.Notice_Flash.Stop();
               this.Notice_Flash.Update();
           }));
        }

        private void Log(string str)
        {
            using (StreamWriter fs = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
            {
                fs.WriteLine(str);
            }
        }
        /************************************************************************/
        /* Edit by Ron.shen 2014-06-10  修改通知播出逻辑，主要为了物业通知可能同时间播放的不止一条,返回列表以后轮流播出
         * 主要修改函数：ChoseNoticeType
         * 增加对象：NoticeList:需要播出的通知列表
         * 修改点：先判断通知列表是否为空，如果为空则从数据库重新获取通知列表，如果不为空则循环，在循环中判断时间：合适：播放并且移除以后break，不合适：直接移除且continue;
        /************************************************************************/

        static List<NoticeModel> NoticeList = new List<NoticeModel>();
        public void ChoseNoticeType()
        {
            if (NoticeList.Count == 0)
            {
                NoticeList = Mb.GetNoticeModel();
                //Log(DateTime.Now + ":查询总数为" + NoticeList.Count);
            }
            //Log(DateTime.Now + ":集合总数为" + NoticeList.Count);
            if (NoticeList != null && NoticeList.Count > 0)
            {
                NoticeModel currentNoticeModel = NoticeList[0];
                NoticeModel noticemodel = NoticeList[0];
                //foreach (NoticeModel noticemodel in NoticeList)
                //{
                // currentNoticeModel = noticemodel;
                if (noticemodel.StarTime <= DateTime.Now && noticemodel.EndTime >= DateTime.Now)
                {
                    if (noticemodel != null)
                    {
                        switch (noticemodel.Type)
                        {
                            case 1:
                                HidGrid();
                                this.txt_Notice.Text = noticemodel.Content;
                                this.grid_Write.Visibility = Visibility.Visible;
                                this.btn_Close.Visibility = Visibility.Visible;
                                break;
                            case 2:
                                HidGrid();
                                FullScreen = noticemodel.FullScreen;
                                BitmapImage image = new BitmapImage(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Image\" + noticemodel.Path), UriKind.Absolute));
                                Log(DateTime.Now + ":现在播放的图片为" + noticemodel.Path);
                                this.NoticeImage.Source = image;
                                this.NoticeImage.Stretch = Stretch.Fill;
                                ControlScreen(this.grid_Image);
                                this.grid_Image.Visibility = Visibility.Visible;
                                //this.btn_Close.Visibility = Visibility.Visible;
                                break;
                            case 3:
                                HidGrid();
                                FullScreen = noticemodel.FullScreen;
                                this.NoticeVideo.Stretch = Stretch.Fill;
                                this.NoticeVideo.Source = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Video\" + noticemodel.Path), UriKind.RelativeOrAbsolute);
                                this.NoticeVideo.Play();
                                ControlScreen(this.grid_Video);
                                this.grid_Video.Visibility = Visibility.Visible;
                                break;
                            case 4:
                                HidGrid();
                                FullScreen = noticemodel.FullScreen;
                                if (FullScreen)
                                { this.pup_Close.IsOpen = false; }
                                else
                                { this.pup_Close.IsOpen = true; }
                                this.Notice_Flash.Movie = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Flash\" + noticemodel.Path);
                                ControlScreen(this.grid_Flash);
                                this.grid_Flash.Visibility = Visibility.Visible;
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        this.Visibility = Visibility.Collapsed;
                    }

                    //break;
                }
                else
                {
                    //continue;
                }
                // }

                if (currentNoticeModel != null)
                {
                    //NoticeList.Remove(currentNoticeModel);
                    NoticeList.RemoveAt(0);
                }
                //Log(DateTime.Now + ":集合总数为" + NoticeList.Count);


            }
            //NoticeModel noticemodel = new NoticeModel();
            //noticemodel = Mb.GetNoticeModel();

        }



        private void HidGrid()
        {
            this.grid_Image.Visibility = Visibility.Collapsed;
            this.grid_Write.Visibility = Visibility.Collapsed;
            this.grid_Video.Visibility = Visibility.Collapsed;
            this.grid_Flash.Visibility = Visibility.Collapsed;
            //this.grid_Flash.Children.Remove(
            // this.Notice_Flash.Stop();
        }

        public void CloseFlash()
        {
            this.Notice_Flash.Stop();
            this.Notice_Flash.Movie = null;
            // this.Notice_Flash.Dispose();
        }


        DateTime lastTouchTime = DateTime.Now;
        int IntervalTime = 1;
        private void NoticeVideo_TouchDown(object sender, TouchEventArgs e)
        {
            TimeSpan ts = DateTime.Now - lastTouchTime;
            lastTouchTime = DateTime.Now;
            if (ts.TotalSeconds > IntervalTime)
            {
                return;
            }
            lastTouchTime = lastTouchTime.AddSeconds(-3);
            if (FullScreen)
            {
                this.btn_Close.Visibility = Visibility.Visible;
                Grid.SetColumn(this.grid_Video, 1);
                Grid.SetRow(this.grid_Video, 1);
                Grid.SetRowSpan(this.grid_Video, 1);
                Grid.SetColumnSpan(this.grid_Video, 1);
                FullScreen = false;
            }
            else
            {
                this.btn_Close.Visibility = Visibility.Collapsed;
                Grid.SetColumn(this.grid_Video, 0);
                Grid.SetRow(this.grid_Video, 0);
                Grid.SetRowSpan(this.grid_Video, 3);
                Grid.SetColumnSpan(this.grid_Video, 3);
                FullScreen = true;
            }
        }


        private void ControlScreen(Grid grid)
        {

            if (FullScreen)
            {
                this.btn_Close.Visibility = Visibility.Collapsed;
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, 0);
                Grid.SetRowSpan(grid, 3);
                Grid.SetColumnSpan(grid, 3);
                FullScreen = false;
            }
            else
            {
                this.btn_Close.Visibility = Visibility.Visible;
                Grid.SetColumn(grid, 1);
                Grid.SetRow(grid, 1);
                Grid.SetRowSpan(grid, 1);
                Grid.SetColumnSpan(grid, 1);
                FullScreen = true;
            }
        }

        private void NoticeImage_TouchDown(object sender, TouchEventArgs e)
        {
            TimeSpan ts = DateTime.Now - lastTouchTime;
            lastTouchTime = DateTime.Now;
            if (ts.TotalSeconds > IntervalTime)
            {
                return;
            }
            lastTouchTime = lastTouchTime.AddSeconds(-3);
            if (FullScreen)
            {
                this.btn_Close.Visibility = Visibility.Visible;
                Grid.SetColumn(this.grid_Image, 1);
                Grid.SetRow(this.grid_Image, 1);
                Grid.SetRowSpan(this.grid_Image, 1);
                Grid.SetColumnSpan(this.grid_Image, 1);
                FullScreen = false;
            }
            else
            {
                this.btn_Close.Visibility = Visibility.Collapsed;
                Grid.SetColumn(this.grid_Image, 0);
                Grid.SetRow(this.grid_Image, 0);
                Grid.SetRowSpan(this.grid_Image, 3);
                Grid.SetColumnSpan(this.grid_Image, 3);
                FullScreen = true;
            }
        }
    }
}
