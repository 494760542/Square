using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Data;
using SOHO.Company.DAL;
using SOHO.Company.Model;
using System.Data.SqlClient;

namespace SOHO.Company.BLL
{
    class CompanyBLL
    {
        //SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        //private static string connectionString = "Data Source=222.73.236.182;Initial Catalog=SOHOBuilding_New;uid=sa;pwd=wyl150709";
        //SqlConnection sqlconnection = new SqlConnection(connectionString);
        public ObservableCollection<CompanyModel> GetCompanyList()
        {
            ObservableCollection<CompanyModel> list = new ObservableCollection<CompanyModel>();

            string buildStr = GetConfigBuild();
            //////////////////////////////////////////////////////////////////////////
            //Edit by Ron.shen 修改SQL语句,将查出来的公司集合按楼栋和房间名排序后输出

            //////////////////////////////////////////////////////////////////////////

            // string sql = string.Format("select ID,CompanyName_CN,CompanyName_EN,RoomNum,Building,Introduction,Floor from T_Company {0} ORDER BY Floor,CAST(RoomNum AS int)", buildStr);
            string sql = string.Format("select ID,CompanyName,EnglishName,RoomNum,BeamNum,CompanyInfo,FloorNum from dt_Company {0} ORDER BY FloorNum,CAST(RoomNum AS int)", buildStr);
            DataSet ds = SQLiteHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CompanyModel cm = new CompanyModel();
                    cm.CompanyID = Int32.Parse(dr["ID"].ToString());
                    cm.CompanyName_CN = dr["CompanyName"].ToString();
                    cm.CompanyName_EN = dr["EnglishName"].ToString();
                    cm.Content = dr["CompanyInfo"].ToString();
                    //string room = dr["BeamNum"].ToString() + dr["RoomNum"].ToString();
                    string room =dr["RoomNum"].ToString();
                    if (room.Contains("F"))
                    {
                        room = dr["BeamNum"].ToString() +dr["FloorNum"].ToString()+ dr["RoomNum"].ToString();
                    }
                    cm.RoomNum=room;
                    list.Add(cm);
                }
            }

            return list;
        }
        /// <summary>
        /// 获取配置文件中设置的数据显示项

        /// </summary>
        /// <returns></returns>
        private string GetConfigBuild()
        {
            try
            {
                string buildStr = System.Configuration.ConfigurationManager.AppSettings["Building"];
                string DeviceId = System.Configuration.ConfigurationManager.AppSettings["DeviceId"];
                if (string.IsNullOrEmpty(buildStr)) return string.Empty;
                return string.Format(" where BeamNum='{0}' and DeviceId="+DeviceId, buildStr);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
