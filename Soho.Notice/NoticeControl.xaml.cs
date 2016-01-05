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
using SOHO.Notice.BLL;
using SOHO.Notice.Model;
using System.Collections.ObjectModel;

namespace SOHO.Notice
{
    /// <summary>
    /// Interaction logic for NoticeControl.xaml
    /// </summary>
    public partial class NoticeControl : BaseUserControl
    {
        public NoticeControl()
        {
            InitializeComponent();
            this.ChildNotice.ReturnHomeEvent += new Notic.NoticeChild.ReturnHomeDelegate(ChildNotice_ReturnHomeEvent);
            this.ChildNotice.Visibility = Visibility.Collapsed;
            this.grid_noticelist.Visibility = Visibility.Visible;
            this.listbox_Notice.SelectedIndex = -1;
        }

        void ChildNotice_ReturnHomeEvent()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ChildNotice.Visibility = Visibility.Collapsed;
                this.grid_noticelist.Visibility = Visibility.Visible;
                this.listbox_Notice.SelectedIndex = -1;
            }));
            
        }

        int Index = 0;
        int Length = 6;
        NoticeBLL Nbll = new NoticeBLL();
        ObservableCollection<NoticeModel> list = new ObservableCollection<NoticeModel>();

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            list = Nbll.GetNoticeList();
            this.listbox_Notice.ItemsSource = GetListValue(0, Length);
        }

        private void but_Next_Click(object sender, RoutedEventArgs e)
        {
            if ((Index + 1) * Length <= list.Count)
            {
                this.listbox_Notice.ItemsSource = GetListValue(++Index, Length);
            }
        }

        private void but_Pre_Click(object sender, RoutedEventArgs e)
        {
            if (Index > 0)
            {
                this.listbox_Notice.ItemsSource = GetListValue(--Index, Length);
            }
        }

        private void but_Return_Click(object sender, RoutedEventArgs e)
        {
            this.listbox_Notice.ItemsSource = GetListValue(0, Length);
            Index = 0;
        }

        private ObservableCollection<NoticeModel> GetListValue(int index, int length)
        {
            ObservableCollection<NoticeModel> getlist = new ObservableCollection<NoticeModel>();
            for (int n = index * length; n < (list.Count > (index + 1) * length ? (index + 1) * length : list.Count); n++)
            {
                NoticeModel noticemodel = new NoticeModel();
                noticemodel = list[n];
                getlist.Add(noticemodel);
            }
            return getlist;
        }

        public override void RefreshData()
        {
            list = Nbll.GetNoticeList();
            this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      this.listbox_Notice.ItemsSource = GetListValue(0, Length);
                  }));
        }

        private void listbox_Notice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      NoticeModel noticemodel = listbox_Notice.SelectedItem as NoticeModel;
                      if (noticemodel != null)
                      {
                          this.grid_noticelist.Visibility = Visibility.Collapsed;
                          this.ChildNotice.DataContext = noticemodel;
                          this.ChildNotice.Visibility = Visibility.Visible;
                      }
                  }));
        }

        public override void ShiftUI()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ChildNotice.Visibility = Visibility.Collapsed;
                this.grid_noticelist.Visibility = Visibility.Visible;
                RefreshData();
                this.listbox_Notice.SelectedIndex = -1;
            }));
            
        }



    }
}
