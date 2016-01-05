using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOHO.MainWindow.Model
{
    class NoticeModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string NoticeTime { get; set; }
        public string Path { get; set; }
        public int Using { get; set; }
        public int Type { get; set; }
        public bool FullScreen { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
