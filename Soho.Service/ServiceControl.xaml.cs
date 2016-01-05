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
using SOHO.Service.Model;
using SOHO.Service.BLL;

namespace SOHO.Service
{
    /// <summary>
    /// Interaction logic for ServiceControl.xaml
    /// </summary>
    public partial class ServiceControl : BaseUserControl
    {
        public ServiceControl()
        {
            InitializeComponent();
            InitUI();
        }

        ServiceBLL bl = new ServiceBLL();
        private void InitUI()
        {
//            this.txt_ServiceContent.Text = @"      客户服务内容包括：负责处理开票请求、账户维护、服务分派、时间安排、一般信息。客户服务通常通过电话进行，但也可以通过电

//子邮件、传真、自服务活邮件进行，它包括：普通服务，现场调研、产品设计、提供产品说明书、提供咨询服务等。客户服务内容包括：
//负责处理开票请求、账户维护、服务分派、时间安排、一般信息。客户服务通常通过电话进行，但也可以通过电子邮件、传真、自服务活

//邮件进行，它包括：普通服务，现场调研、产品设计、提供产品说明书、提供咨询服务等。";
//            this.txt_ServiceContent.Width = 1500;
            ServiceModel servicemodel = new ServiceModel();
            servicemodel = bl.GetServiceMode();
            this.txt_ServiceContent.Text = servicemodel.ServiceContent;
            //this.txt_Serviceing.Text = servicemodel.Serviceing;
            //this.txt_Serviced.Text = servicemodel.Serviced;
        }
    }
}
