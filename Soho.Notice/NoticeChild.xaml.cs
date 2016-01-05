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

namespace SOHO.Notic
{
    /// <summary>
    /// NoticeChild.xaml 的交互逻辑
    /// </summary>
    public partial class NoticeChild : UserControl
    {
        public NoticeChild()
        {
            InitializeComponent();
        }

        public delegate void  ReturnHomeDelegate();
        public event ReturnHomeDelegate ReturnHomeEvent;

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnHomeEvent();
        } 

        
        
    }
}
