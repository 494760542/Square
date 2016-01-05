using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SOHO.Notice.Model;
using SOHO.Notice.DAL;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;

namespace SOHO.Notice.BLL
{

    class NoticeBLL
    {
        SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        public ObservableCollection<NoticeModel> GetNoticeList()
        {
            ObservableCollection<NoticeModel> list = new ObservableCollection<NoticeModel>();
            string sql = "select ID,Content,Title,NoticeTime,Author from T_Notice where Type=1";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    NoticeModel noticemodel = new NoticeModel();
                    noticemodel.ID = Int32.Parse(dr["ID"].ToString());
                    noticemodel.NoticeTitle = dr["Title"].ToString();
                    noticemodel.NoticeTime = dr["NoticeTime"].ToString();
                    noticemodel.NoticeContent = dr["Content"].ToString();
                    noticemodel.NoticeAuthor = dr["Author"].ToString();
                    list.Add(noticemodel);
                }
            }
             return list;   
        }
    }
}
