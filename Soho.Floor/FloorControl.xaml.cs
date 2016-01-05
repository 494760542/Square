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
using SOHO.Floor.BLL;
using System.Collections.ObjectModel;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Core;
using Microsoft.Surface.Presentation.Controls;
using SOHO.Floor.Model;
using SOHO.Floor.Common;

namespace SOHO.Floor
{
    /// <summary>
    /// Interaction logic for FloorControl.xaml
    /// </summary>
    public partial class FloorControl : BaseUserControl
    {
        public FloorControl()
        {
            InitializeComponent();
            this.Child_Company.ReturnHomeEvent += new FloorChid.ReturnHomeDelegate(Child_Company_ReturnHomeEvent);
            EnderFloor();
            ChoseBuild();
        }


        private void EnderFloor()
        {
            this.Child_Company.Visibility = Visibility.Collapsed;
            this.grid_Company.Visibility = Visibility.Visible;
            this.listbox_Floor.SelectedIndex = -1;
        }

        void Child_Company_ReturnHomeEvent()
        {
            EnderFloor();
        }

        FloorBLL fb = new FloorBLL();
        int floor;
        string build;
        List<string> buildlist = new List<string>();
        ObservableCollection<FloorModel> list = new ObservableCollection<FloorModel>();

        //////////////////////////////////////////////////////////////////////////
        //2014-01-07
        //Edit by Ron.shen 修改加载楼层信息逻辑，将原来直接加载所有楼层列表修改为
        //                 如果超过一定数量楼层时分页显示
        //////////////////////////////////////////////////////////////////////////
        List<int> floorAlllist = new List<int>();
        int CurrentPage = 0;
        int step = 15;
        private void LoadFloor()
        {
            ObservableCollection<SurfaceButton> ButList = new ObservableCollection<SurfaceButton>();

            Uri uri = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\icon_floor01.png"), UriKind.RelativeOrAbsolute);
            BitmapImage bimg = new BitmapImage(uri);
            SplitListHelper.ClearList();
            SplitListHelper.SplitDic(floorAlllist, step);
            List<int> floorlist = SplitListHelper.GetChildList(CurrentPage);
            if (SplitListHelper.SplitList.Count > 1)
            {
                AddFloorButton(ButList, "<<", bimg);
            }
            for (int i = 0; i < floorlist.Count; i++)
            {
                floor = floorlist[0];
                string ContentValue = "";

                if (Convert.ToInt32(floorlist[i].ToString().Trim()) > 0)
                {
                    ContentValue = @"F" + floorlist[i].ToString().Trim();
                }
                else if (Convert.ToInt32(floorlist[i].ToString().Trim()) < 0)
                {
                    ContentValue = @"B" + Math.Abs(Convert.ToInt32(floorlist[i].ToString()));
                }
                else
                {
                    ContentValue = @"G";
                }
                AddFloorButton(ButList, ContentValue, bimg);
            }
            if (SplitListHelper.SplitList.Count > 1)
            {
                AddFloorButton(ButList, ">>", bimg);
            }
            this.lv_Floor.ItemsSource = ButList;
            LoadCompany();
            
            SurfaceButton but = null;
            if (SplitListHelper.SplitList.Count > 1)
            {
                but = ButList.ToList()[1] as SurfaceButton;
            }
            else
            {
                but = ButList.ToList()[0] as SurfaceButton;
            }

            dib_Click(but, new RoutedEventArgs());
        }


        public void AddFloorButton(ObservableCollection<SurfaceButton> ButList, string ContentValue, BitmapImage bimg)
        {
            SurfaceButton dib = new SurfaceButton();
            dib.Background = new ImageBrush(bimg);
            //dib.Background = new SolidColorBrush(Colors.Green);
            dib.Foreground = Brushes.White;
            dib.FontSize = 20;
            dib.Height = 50;
            dib.Width = 50;
            //Label lb = new Label();
            //lb.Background = new SolidColorBrush(Colors.Red);
            //lb.Content = ContentValue;
            dib.HorizontalContentAlignment = HorizontalAlignment.Center;
            dib.VerticalContentAlignment = VerticalAlignment.Center;
            dib.HorizontalAlignment = HorizontalAlignment.Center;
            dib.VerticalAlignment = VerticalAlignment.Center;
            dib.Content = ContentValue;
            dib.IsTabStop = false;

            dib.Click += new RoutedEventHandler(dib_Click);
            ButList.Add(dib);
        }

        private void ChoseBuild()
        {
            buildlist = fb.GetBulidList();
            //build = buildlist[0];
            build= System.Configuration.ConfigurationManager.AppSettings["Building"];
            floorAlllist = fb.GetFloorList(build);
            CurrentPage = 0;
            LoadFloor();
            //this.lab_tent.Content = build + "栋";
            this.lab_tent.Content ="本栋";
        }

        private void ShiftBuild()
        {
            if (buildlist.Count == 2)
            {
                if (build == buildlist[0])
                {
                    this.lab_tent.Content = buildlist[1] + "栋";
                    build = buildlist[1];
                }
                else
                {
                    this.lab_tent.Content = buildlist[0] + "栋";
                    build = buildlist[0];
                }
                floorAlllist = fb.GetFloorList(build);
                CurrentPage = 0;
                LoadFloor();
            }
            else
                return;
        }

        void dib_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\icon_floor.png"), UriKind.RelativeOrAbsolute);
            Uri uriClick = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\icon_floor01.png"), UriKind.RelativeOrAbsolute);
            BitmapImage bimg = new BitmapImage(uri);
            BitmapImage bimgCilck = new BitmapImage(uriClick);
            SurfaceButton surfacebutton = sender as SurfaceButton;
            ObservableCollection<SurfaceButton> ButList = this.lv_Floor.ItemsSource as ObservableCollection<SurfaceButton>;
            if (ButList != null)
            {
                foreach (SurfaceButton item in ButList)
                {
                    item.Background = new ImageBrush(bimg);
                    //item.Background = new SolidColorBrush(Colors.Green);
                }
            }
            surfacebutton.Background = new ImageBrush(bimgCilck);
            //Label lb = surfacebuttton.Content as Label;
            char str = surfacebutton.Content.ToString()[0];
            switch (str)
            {
                case 'G':
                    floor = 0;
                    break;
                case '<':

                    //////////////////////////////////////////////////////////////////////////
                    //增加前翻页和后翻页的逻辑判断
                    //////////////////////////////////////////////////////////////////////////
                    CurrentPage = (CurrentPage - 1) < 0 ? 0 : (CurrentPage - 1);
                    LoadFloor();
                    break;
                case '>':
                    CurrentPage = (CurrentPage + 1) >= SplitListHelper.SplitList.Count ? SplitListHelper.SplitList.Count - 1 : (CurrentPage + 1);
                    LoadFloor();
                    break;
                default:
                    //Label lb = surfacebuttton.Content as Label;
                    floor = Int32.Parse(surfacebutton.Content.ToString().Remove(0, 1));
                    if (str == 'B')
                    {
                        floor *= -1;
                    }
                    break;
            }
            LoadCompany();
        }



        private void LoadCompany()
        {

            list = fb.GetCompanyList(floor, build);
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      listbox_Floor.ItemsSource = list;
                  }));
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                // Handle exceptions as needed.
            }
        }

        private void listbox_Floor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FloorModel floormodel = this.listbox_Floor.SelectedItem as FloorModel;
            if (floormodel != null)
            {
                this.grid_Company.Visibility = Visibility.Collapsed;
                this.Child_Company.DataContext = floormodel;
                this.Child_Company.Visibility = Visibility.Visible;
                //MessageBox.Show(bw.CurrentUserControlName);
            }
        }

        private void but_Enter_Click(object sender, RoutedEventArgs e)
        {
            ShiftBuild();
        }

        public override void RefreshData()
        {
            list = fb.GetCompanyList(floor, build);
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      listbox_Floor.ItemsSource = list;
                  }));
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                // Handle exceptions as needed.
            }
        }


    }
}
