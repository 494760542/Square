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
using Microsoft.Surface.Presentation;
using SOHO.Company.Model;
using SOHO.Company.DAL;
using SOHO.Company.BLL;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Configuration;


namespace SOHO.Company
{
    /// <summary>
    /// Interaction logic for CompanyControl.xaml
    /// </summary>
    public partial class CompanyControl : BaseUserControl
    {
        public CompanyControl()
        {
            InitializeComponent();
            numFonsize = ConvertToDouble(GetFontSize("NumFonSize"), 26);
            cnFonsize = ConvertToDouble(GetFontSize("CnFonSize"), 24);
            enFonsize = ConvertToDouble(GetFontSize("EnFonSize"), 22);
            SetFonSize("FonSizeNum", numFonsize);
            SetFonSize("FonSizeCn", cnFonsize);
            SetFonSize("FonSizeEn", enFonsize);
            this.Child_Company.ReturnHomeEvent += new SOHO.CompanyControl.CompanyChild.ReturnHomeDelegate(Child_Company_ReturnHomeEvent);
            EnderCompany();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        double numFonsize, cnFonsize, enFonsize;
        CompanyBLL cb = new CompanyBLL();
        ObservableCollection<CompanyModel> list = new ObservableCollection<CompanyModel>();
        private void EnderCompany()
        {
            this.Child_Company.Visibility = Visibility.Collapsed;
            this.grid_Company.Visibility = Visibility.Visible;
            this.MainSurfaceListBox.SelectedIndex = -1;
            int fontsize = ConvertToInt(GetFontSize("ShiftUI"), 24);
            
        }
        
        void Child_Company_ReturnHomeEvent()
        {
            EnderCompany();
            BaseMainWindow bw = FindParent<BaseMainWindow>(this.VisualPanel);
            bw.CurrentUserControlName = bw.CurrentUserControlName.Replace("_child","");
        }

        System.Timers.Timer timer = new System.Timers.Timer(3000);
        private void MainSurfaceListBox_TouchDown(object sender, TouchEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                timer.Stop();
                //LoopPanelVertical.PauseTimer();
                PauseLoopTime();
            }));
           
            
        }

        private void PauseLoopTime()
        {
            LoopPanelVertical textblackConnPointNum = (LoopPanelVertical)(MainSurfaceListBox.Template.FindName("loop", MainSurfaceListBox));
            textblackConnPointNum.PauseTimer();
        }

        private void MainSurfaceListBox_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                timer.Stop();
                StartLoopTime();
            }));
           
            
        }

        private void StartLoopTime()
        {
            LoopPanelVertical textblackConnPointNum = (LoopPanelVertical)(MainSurfaceListBox.Template.FindName("loop", MainSurfaceListBox));
            if (textblackConnPointNum == null) return;
            textblackConnPointNum.StartTimer();
        }

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            list = cb.GetCompanyList();
            try
            {
                MainSurfaceListBox.ItemsSource = list;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                // Handle exceptions as needed.
            }
        }
        private string GetFontSize(string config)
        {
            string fontSize = System.Configuration.ConfigurationManager.AppSettings[config];
            return  fontSize;
        }

        private void SetFonSize(string TxtStyle, double fonSize)
        {
            Style style = (Style)this.Resources[TxtStyle];
            Setter setter = new Setter();
            setter.Property = FontSizeProperty;
            setter.Value = fonSize;
            style.Setters.Add(setter);

        }

        private void MainSurfaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CompanyModel companymodel = this.MainSurfaceListBox.SelectedItem as CompanyModel;
           
            if (companymodel != null)
            {
                if (string.IsNullOrEmpty(companymodel.Content)) return;
                this.grid_Company.Visibility = Visibility.Collapsed;
                this.Child_Company.DataContext = companymodel;
                this.Child_Company.Visibility = Visibility.Visible;
                BaseMainWindow bw = FindParent<BaseMainWindow>(this.VisualPanel);
                bw.CurrentUserControlName += "_child";
                //MessageBox.Show(bw.CurrentUserControlName);
            }
        }

        public static T FindParent<T>(DependencyObject d) where T : DependencyObject
        {
            DependencyObject parent = d;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent != null && ((parent.GetType() == typeof(T))||parent is T))
                {
                    return parent as T;
                }
            }
            return parent as T;
        }

        public override void ShiftUI()
        {
            
            EnderCompany();
            RefreshData();
            StartLoopTime();
        }

        public override void RefreshData()
        {
            list = cb.GetCompanyList();
            StartLoopTime();
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       MainSurfaceListBox.ItemsSource = list;
                   }));
               
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                // Handle exceptions as needed.
            }
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="orgStr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private double ConvertToDouble(string orgStr, double defaultValue)
        {
            double result = 0.0;
            if (!double.TryParse(orgStr, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="orgStr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int ConvertToInt(string orgStr, int defaultValue)
        {
            int result = 0;
            if (!int.TryParse(orgStr, out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
