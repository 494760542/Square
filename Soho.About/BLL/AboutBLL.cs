using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SOHO.About.Model;
using SOHO.About.DAL;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;

namespace SOHO.About.BLL
{

    class AboutBLL
    {
        //SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        public AboutModel GetAboutMode()
        {
            AboutModel aboutmodel = new AboutModel();
            string DeviceId = System.Configuration.ConfigurationManager.AppSettings["DeviceId"];
            string sql = "select FilePath,Content,Phone,WebSit,Address,BusRoute from B_PropertyIntroduce where DeviceId="+DeviceId;
            DataSet ds = SQLiteHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                aboutmodel.AboutImage = ds.Tables[0].Rows[0]["FilePath"].ToString();
                aboutmodel.AboutContent = ds.Tables[0].Rows[0]["Content"].ToString();
                aboutmodel.AboutTelephone = ds.Tables[0].Rows[0]["Phone"].ToString();
                aboutmodel.AboutInternet = ds.Tables[0].Rows[0]["WebSit"].ToString();
                aboutmodel.AboutAdress = ds.Tables[0].Rows[0]["Address"].ToString();
                aboutmodel.AboutBus = ds.Tables[0].Rows[0]["BusRoute"].ToString();
            }
            return aboutmodel;   
        }
    }
}
