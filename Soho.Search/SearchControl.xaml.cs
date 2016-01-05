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
using SOHO.Search.BLL;
using SOHO.Search.Model;
using System.Collections.ObjectModel;
using UskyPlugsFrame.BaseShow;

namespace SOHO.Search
{
    //////////////////////////////////////////////////////////////////////////
    //Edit by Ron.shen 修改此界面,去除之前的查询界面，修改为随输入公司名称变动而筛选公司列表机制

    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : BaseUserControl
    {
        public SearchControl()
        {
            InitializeComponent();
            this.SearchCompany.ReturnHomeEvent += new CompanyChild.ReturnHomeDelegate(SearchCompany_ReturnHomeEvent);
            LoadListData("");
        }

        void SearchCompany_ReturnHomeEvent()
        {
            EnderCompany();
        }

        private void EnderCompany()
        {
            this.SearchCompany.Visibility = Visibility.Collapsed;
            this.grid_Company.Visibility = Visibility.Visible;
            this.listbox_Search.SelectedIndex = -1;
        }

        ObservableCollection<CompanyModel> companylist;
        SearchBLL searchbll = new SearchBLL();
        public delegate void ReturnHomeDelegate();
        public event ReturnHomeDelegate ReturnHomeEvent;
        private void but_Enter_Click(object sender, RoutedEventArgs e)
        {
            // this.Keyboard.IsOpen = true;
            //ReturnHomeEvent();
            bool isopen = this.Keyboard.IsOpen;
            this.txt_Condition.Text = "";
            this.Keyboard.IsOpen = isopen;
        }

        public void LoadListData(string sql)
        {
            CompanyModel companymodel = new CompanyModel();
            companylist = searchbll.GetCompanyList(sql);
            this.listbox_Search.ItemsSource = companylist;
            EnderCompany();
        }

        private void listbox_Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CompanyModel companymodel = this.listbox_Search.SelectedItem as CompanyModel;
                if (companymodel != null)
                {
                    this.grid_Company.Visibility = Visibility.Collapsed;
                    this.SearchCompany.DataContext = companymodel;
                    this.SearchCompany.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void SurfaceTextBox_TouchDown(object sender, TouchEventArgs e)
        {
            this.Keyboard.IsOpen = true;
        }

        private void txt_Condition_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Keyboard.IsOpen = true;
            string str_sql = "";
            StringBuilder sql_build = new StringBuilder();
            if (!string.IsNullOrEmpty(txt_Condition.Text))
            {
                //////////////////////////////////////////////////////////////////////////
                //Edit by Ron.shen 嵌入特殊指令关闭程序，解决部署之后无键盘无法关闭程序问题
                //////////////////////////////////////////////////////////////////////////
                if (txt_Condition.Text.ToLower().Equals("closethisapp"))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Application.Current.Shutdown();
                    }));
                }
                sql_build.Append("Pingying Like '%" + txt_Condition.Text.Trim() + @"%'");
                sql_build.Append("or EnglishName Like '%" + txt_Condition.Text.Trim() + @"%'");
            }
            if (!string.IsNullOrEmpty(sql_build.ToString()))
            {
                str_sql = "AND " + sql_build.ToString();
            }
            LoadListData(str_sql);
        }

        private void txt_Condition_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public override void ShiftUI()
        {
            this.txt_Condition.Text = string.Empty;
            this.Keyboard.IsOpen = false;
        }

        public override void RefreshData()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    companylist = searchbll.GetCompanyList("");
                    this.listbox_Search.ItemsSource = companylist;
                }));
        }
    }
}
