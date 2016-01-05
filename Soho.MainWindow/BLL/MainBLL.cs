using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Data;
using SOHO.MainWindow.Model;
using SOHO.MainWindow.DAL;
using System.Drawing;

namespace SOHO.MainWindow.BLL
{
    class MainBLL
    {
        SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        
        public ConfigModel GetConfig()
        {
            ConfigModel cofigmodel=new ConfigModel();
            string sql = @"select ID,IdleTime,NoticeShowTime from T_ConfigTime";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cofigmodel.ID = Int32.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                cofigmodel.IdleTime = Int32.Parse(ds.Tables[0].Rows[0]["IdleTime"].ToString());
                cofigmodel.NoticeShowTime = Int32.Parse(ds.Tables[0].Rows[0]["NoticeShowTime"].ToString());
            }
            return cofigmodel;
        }
       /************************************************************************/
        /* Edit by Ron.shen 2014-06-10  修改返回通知列表，主要为了物业通知可能同时间播放的不止一条,返回列表以后轮流播出
         * 主要修改函数：GetNoticeModel、ChoseNotice
         * 修改点：将返回的单个实体对象改成对象集合
        /************************************************************************/
        public List<NoticeModel> GetNoticeModel()
        {
            List<NoticeModel> noticelist = new List<NoticeModel>();
            string sql = @"select ID,Type,Used,Content,Path,Title,NoticeTime,FullScreen,StartTime,EndTime from T_Notice ";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count;i++ )
            {
                NoticeModel notcemodel = new NoticeModel();
                notcemodel.ID = Int32.Parse(ds.Tables[0].Rows[i]["ID"].ToString());
                notcemodel.Using = Int32.Parse(ds.Tables[0].Rows[i]["Used"].ToString());
                notcemodel.Type = Int32.Parse(ds.Tables[0].Rows[i]["Type"].ToString());
                notcemodel.Path = ds.Tables[0].Rows[i]["Path"].ToString();
                notcemodel.Content = ds.Tables[0].Rows[i]["Content"].ToString();
                notcemodel.Title = ds.Tables[0].Rows[i]["Title"].ToString();
                notcemodel.NoticeTime = ds.Tables[0].Rows[i]["NoticeTime"].ToString();
                string starttime=string.Empty;
                  starttime  = ds.Tables[0].Rows[i]["StartTime"].ToString();
                  string endtime = string.Empty;
                   endtime = ds.Tables[0].Rows[i]["EndTime"].ToString();
                if (starttime != string.Empty && starttime != null)
                {
                    notcemodel.StarTime = DateTime.Parse(starttime);
                }
                if (endtime != string.Empty && endtime != null)
                {
                    notcemodel.EndTime = DateTime.Parse(ds.Tables[0].Rows[i]["EndTime"].ToString());
                }
                int full = Int32.Parse(ds.Tables[0].Rows[i]["FullScreen"].ToString());
                if (full == 1)
                {
                    notcemodel.FullScreen = true;
                }
                else
                {
                    notcemodel.FullScreen = false;
                }
                noticelist.Add(notcemodel);
            }
            return ChoseNotice(noticelist);
        }

        private List<NoticeModel> ChoseNotice(List<NoticeModel> noticelist)
        {
            List<NoticeModel> list = new List<NoticeModel>();
            for (int i = 0; i < noticelist.Count(); i++)
            {
                if (noticelist[i].StarTime <= DateTime.Now && noticelist[i].EndTime >= DateTime.Now)
                {
                    list.Add(noticelist[i]);
                }
            }
            return list;
        }

        public List<MenuModel> GetMenuList()
        {
            List<MenuModel> listmenu = new List<MenuModel>();

            string sql = @"select ID,ClassName,Icon,UserControlIndex from T_Menu order by UserControlIndex";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            for(int n=0;n<ds.Tables[0].Rows.Count;n++)
            {
                MenuModel menumodel = new MenuModel();
                menumodel.ID = Int32.Parse(ds.Tables[0].Rows[n]["ID"].ToString());
                menumodel.ClassName = ds.Tables[0].Rows[n]["ClassName"].ToString();
                menumodel.IconPath = ds.Tables[0].Rows[n]["Icon"].ToString();
                menumodel.UserControlIndex = Int32.Parse(ds.Tables[0].Rows[n]["UserControlIndex"].ToString());
                listmenu.Add(menumodel);
            }
            return listmenu;
        }

        public  int GetUpdateMessage()
        {
            int flag = 0;
            string sql = @"select UpdateFlag from T_Update";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            if ((ds.Tables[0].Rows.Count == 0) || ds == null)
                return flag;
            {
                flag = Int32.Parse(ds.Tables[0].Rows[0]["UpdateFlag"].ToString());
            }
            return flag;
        }

        public void SetUpdateMessage()
        {
            string sql = @"UPDATE  T_Update set [UpdateFlag]=0";
            SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
        }
    }
}
