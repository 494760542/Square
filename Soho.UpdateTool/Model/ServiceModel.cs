using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.Model
{
    class ServiceModel
    {
        public int ServiceID { get; set; }
        public string ServiceContent { get; set; }
        public string ServiceSale { get; set; }
        public string ServiceAfterSale { get; set; }
        public string SetUpdateTime { get; set; }
    }
}
