using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOHO.Notice.Model
{
    class NoticeModel
    {
        public int ID { get; set; }
        public string NoticeTitle { get; set; }
        public string NoticeTime { get; set; }
        public string NoticeContent { get; set; }
        public string NoticeAuthor { get; set; }
    }
}
