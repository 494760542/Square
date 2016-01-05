using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Data;
using SOHO.Search.Model;
using SOHO.Search.DAL;

namespace SOHO.Search.BLL
{
    class SearchBLL
    {
        //SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        public ObservableCollection<CompanyModel> GetCompanyList(string Condition)
        {
            ObservableCollection<CompanyModel> list = new ObservableCollection<CompanyModel>();
            string DeviceId = System.Configuration.ConfigurationManager.AppSettings["DeviceId"];
            string sql = "select ID,CompanyName,EnglishName,RoomNum,BeamNum,FloorNum,CompanyInfo from dt_Company where DeviceId ="+DeviceId+" "  + Condition+ " order by FloorNum,RoomNum";

            DataSet ds = SQLiteHelper.Query(sql);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return list;
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CompanyModel cm = new CompanyModel();
                    cm.CompanyID = Int32.Parse(dr["ID"].ToString());
                    cm.CompanyName_CN = dr["CompanyName"].ToString();
                    cm.CompanyName_EN = dr["EnglishName"].ToString();
                    cm.CompanyRoomNum = dr["RoomNum"].ToString();
                    cm.CompanyBuild = dr["BeamNum"].ToString() + @"栋";
                    cm.CompanyFloor = dr["FloorNum"].ToString();
                    cm.CompanyContent = dr["CompanyInfo"].ToString();
                    list.Add(cm);
                }
            }
            return list;
        }
    }
}
