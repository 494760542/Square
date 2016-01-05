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
using SOHO.About.Model;
using SOHO.About.BLL;
using System.IO;

namespace SOHO.About
{
    /// <summary>
    /// Interaction logic for AboutControl.xaml
    /// </summary>
    public partial class AboutControl : BaseUserControl
    {
        public AboutControl()
        {
            InitializeComponent();
            InitUI();
            //this.txt_Telephone.Text = @"021-31336688";
            //this.txt_Internet.Text = @"www.sohochina.com";
            //this.txt_Adress.Text = @"长宁路中山西路1065号";
            //this.txt_Bus.Text = @"11路 222路 3号线";
        }

        AboutBLL bl = new AboutBLL();
        private void InitUI()
        {
            AboutModel aboutmodel = new AboutModel();
            aboutmodel = bl.GetAboutMode();
            this.txt_Content.Text=aboutmodel.AboutContent;
            this.txt_Telephone.Text=aboutmodel.AboutTelephone;
            this.txt_Internet.Text=aboutmodel.AboutInternet;
            this.txt_Adress.Text=aboutmodel.AboutAdress;
            this.txt_Bus.Text = aboutmodel.AboutBus;

            string internalResourcePath = Directory.GetCurrentDirectory();//保存当前目录
            //string path = internalResourcePath + @"\Resources\"+aboutmodel.AboutImage;
            string path= "http://222.73.236.182:10023" + aboutmodel.AboutImage;
            //if (File.Exists(path))
            //{
            //    this.Picture.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            //}
            this.Picture.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
