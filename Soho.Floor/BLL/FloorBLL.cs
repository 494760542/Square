using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using SOHO.Floor.Model;
using SOHO.Floor.DAL;
using System.Data;

namespace SOHO.Floor.BLL
{
    class FloorBLL
    {
        private string DeviceId = System.Configuration.ConfigurationManager.AppSettings["DeviceId"];
        //SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        public ObservableCollection<FloorModel> GetCompanyList(int index,string bulid)
        {
            ObservableCollection<FloorModel> list = new ObservableCollection<FloorModel>();
            string sql = "select ID,CompanyName,EnglishName,RoomNum,BeamNum,CompanyInfo from dt_Company where DeviceId="+ DeviceId+" and FloorNum=" + index + @" and BeamNum='" + bulid + @"' ORDER BY FloorNum,RoomNum";
            DataSet ds = SQLiteHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    FloorModel fm = new FloorModel();
                    fm.CompanyID = Int32.Parse(dr["ID"].ToString());
                    fm.CompanyName_CN = dr["CompanyName"].ToString();
                    fm.CompanyName_EN = dr["EnglishName"].ToString();
                    fm.Content = dr["CompanyInfo"].ToString();
                    //string room = dr["BeamNum"].ToString() + dr["RoomNum"].ToString();
                    string room = dr["RoomNum"].ToString();
                    if (room.Contains("F"))
                    {
                        room = dr["BeamNum"].ToString() + index.ToString() + dr["RoomNum"].ToString();
                    }
                    fm.RoomNum = room;
                    list.Add(fm);
                }
            }
            return list;
        }

        public List<int> GetFloorList(string build)
        {
            List<int> floorlist = new List<int>();
            string sql = "select FloorNum from dt_Company where DeviceId="+DeviceId+" and BeamNum='" + build+  @"' GROUP BY FloorNum";
            DataSet ds = SQLiteHelper.Query(sql);
            for (int n = 0; n < ds.Tables[0].Rows.Count; n++)
            {
                int floor = Int32.Parse(ds.Tables[0].Rows[n]["FloorNum"].ToString());
                floorlist.Add(floor);
            }
            return floorlist;
        }

        public List<string> GetBulidList()
        {
            List<string> bulidlist = new List<string>();
            string sql = "select BeamNum from dt_Company where DeviceId=" + DeviceId + "  GROUP BY BeamNum ";
            DataSet ds = SQLiteHelper.Query(sql);
            for (int n = 0; n < ds.Tables[0].Rows.Count; n++)
            {
                string floor = ds.Tables[0].Rows[n]["BeamNum"].ToString();
                bulidlist.Add(floor);
            }
            return bulidlist;
        }

     
    }
}
