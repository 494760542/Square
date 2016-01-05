using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.Model
{
    class MenuModel
    {
        public int MenuID { get; set; }
        public string MenuClassName { get; set; }
        public string MenuIcon { get; set; }
        public string MenuUserControlIndex { get; set; }
        public string MenuUpdateTime { get; set; }
        public string MenuDLLPath { get; set; }
    }
}
