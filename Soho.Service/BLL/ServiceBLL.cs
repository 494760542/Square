using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SOHO.Service.Model;
using SOHO.Service.DAL;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;

namespace SOHO.Service.BLL
{

    class ServiceBLL
    {
        SQLiteConnection sqlconnection = new SQLiteConnection(@"Data Source=.\Data.db;Initial Catalog=T_Company;Persist Security Info=True;");
        public ServiceModel GetServiceMode()
        {
            ServiceModel servicemodel = new ServiceModel();
            string sql = "select Sale,Content,AfterSale from T_Service";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlconnection, sql, null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                servicemodel.ServiceContent = ds.Tables[0].Rows[0]["Content"].ToString();
                servicemodel.Serviceing = ds.Tables[0].Rows[0]["Sale"].ToString();
                servicemodel.Serviced = ds.Tables[0].Rows[0]["AfterSale"].ToString();
            }
            return servicemodel;
        }
    }
}
