using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.Model
{
    /************************************************************************/
    /* Add by Ron.shen 强制下载对象实体类
    /************************************************************************/
    class ResourceInfoModel
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        public string FileURL { get; set; }
        /// <summary>
        /// 保存相对路径
        /// </summary>
        public string DownFilePath { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string FileUpdateTime { get; set; }

    }
}
