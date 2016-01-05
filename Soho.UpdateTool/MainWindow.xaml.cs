using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using Decrypto;
using System.Data.SQLite;
using System.Configuration;
using System.Data;
using UpdateTool.DAL;
using UpdateTool.Model;
using System.Data.Common;

namespace UpdateTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //LoadPath = System.Configuration.ConfigurationManager.AppSettings["LoadPath"];
            // Db_Sqlite = new DbHelper(EncryptionString.Decrypto(System.Configuration.ConfigurationManager.AppSettings["SqlitePath"]), DbProvider.Sqlite);//连接sqlite数据库

            Db_Sql = new DbHelper(EncryptionString.Decrypto(System.Configuration.ConfigurationManager.AppSettings["SqlPath"]), DbProvider.Sql);         //连接sql数据
        }

        DbHelper Db_Sqlite = null;
        DbHelper Db_Sql = null;
        ISqlUpdate I_Sql;
        string WebPath = string.Empty;
        string LoadPath = string.Empty;
        StringDecrypto EncryptionString = new StringDecrypto();
        SelectFactory SqlFactory = new SelectFactory();
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            //查找文件路径
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowDialog();
            if (folderBrowser.SelectedPath != string.Empty)
                this.BrowseName.Text = folderBrowser.SelectedPath;

        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            WriteLog("开始执行更新");
            LoadPath = CreateFile();
            UpdateTableData();
            WriteLog("结束执行更新");
        }

        private void UpdateTableData()
        {
            if (!string.IsNullOrEmpty(this.BrowseName.Text.Trim()))
            {
                if (Directory.Exists(this.BrowseName.Text.Trim()))
                {
                   
                    SynchDataSeting();
                    SynchDataDownFile();
                    SynchDataCompany();
                    SynchDataNotice();
                    SynchDataRule();
                    SynchDataService();
                }
                else
                {
                    WriteLog("文件夹路径不对，请重新选择文件");
                }
            }
            else
            {
                WriteLog("请选择文件夹");
            }
        }

        /************************************************************************/
        /*Add by Ron.shen 此处是为了强制下载某些资源文件，主要应用于某些需要偷偷更新用户资源的场景!-_-
        /************************************************************************/

        #region 强制下载需要下载的资源
        private void SynchDataDownFile()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.DownFile);
            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            List<ResourceInfoModel> reslist = GetResourceInfoModel(SqlTable);
            List<string> downFileUpdateSql = new List<string>();
            IList<DbParameter[]> parlist = new List<DbParameter[]>();
            foreach (ResourceInfoModel rm in reslist)
            {
                string WebSit = WebPath + rm.FileURL;
                bool result= LoadSavePath(rm,WebSit);
                
                if (result)
                {
                    //更新已下载
                    DbParameter[] sqlparList = new DbParameter[1];
                    //sqlparList[0] = Db_Sql.CreateDbParameter("@UpdateTime", DateTime.Now);
                    sqlparList[0] = Db_Sql.CreateDbParameter("@ID", rm.ID);
                    //downFileUpdateSql.Add();
                    //parlist.Add(sqlparList);
                    string error = "";
                    int t= Db_Sql.ReExNum(I_Sql.GetSqliteUpdate(), out error, sqlparList);

                }
                
            }
            //int m = Db_Sql.ReExNum(downFileUpdateSql, parlist);
        }
        private List<ResourceInfoModel> GetResourceInfoModel(DataTable table)
        {
            List<ResourceInfoModel> list = new List<ResourceInfoModel>();
            if (table!=null)
            {
                foreach (DataRow dr in table.Rows)
                {
                    ResourceInfoModel rm = new ResourceInfoModel();
                    rm.ID = Convert.ToInt32(dr["Id"].ToString());
                    rm.FileURL = dr["URL"].ToString();
                    rm.DownFilePath = dr["Path"].ToString();
                    list.Add(rm);
                }
            }
           
            return list;
        }
        private bool LoadSavePath(ResourceInfoModel model, string WebSit)
        {
            string SavePath = string.Empty;
            SavePath = model.DownFilePath;
            SavePath = LoadPath + SavePath;
            bool result=  DownLoad(WebSit, SavePath);
            return result;
        }     
      
        #endregion

        #region 公司表数据同步

        private void SynchDataCompany()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Compnay);
            SqlTable = Db_Sqlite.ReSelectdtb(((SqlInterface)I_Sql).GetLatestUpdateTime());
            if (SqlTable.Rows.Count > 0)
            {
                updatetime = SqlTable.Rows[0]["UpdateTime"].ToString();
            }
            CompanyData(updatetime);
        }

        private void CompanyData(string updatetime)
        {
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Compnay);
            DataTable SqlTable = new DataTable();
            DataTable SqliteTable = new DataTable();
            List<int> listsql = new List<int>();
            List<int> listsqlite = new List<int>();
            List<int> difference = new List<int>();
            List<string> SelectList = new List<string>();
            SqlTable = Db_Sql.ReSelectdtb(((SqlInterface)I_Sql).GetDatafromSql(updatetime));
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            Dictionary<int, CompanyModel> sqltablelist = GetSqlcCompany(SqlTable);
            IList<DbParameter[]> parlist = new List<DbParameter[]>();

            for (int j = 0; j < SqliteTable.Rows.Count; j++)
            {
                int id = Int32.Parse(SqliteTable.Rows[j]["ID"].ToString());
                for (int i = 0; i < sqltablelist.Count; i++)
                {
                    if (id == sqltablelist.Keys.ToList()[i])
                    {
                        DbParameter[] DbUpdate = ExecutionSelectCompany(sqltablelist[id]);
                        parlist.Add(DbUpdate);
                        SelectList.Add(I_Sql.GetSqliteUpdate());
                        sqltablelist.Remove(id);
                        i--;
                    }
                }
            }

            foreach (int Select in sqltablelist.Keys)                                               //根据差集进行插入操作
            {
                DbParameter[] DbInsert = ExecutionSelectCompany(sqltablelist[Select]);
                parlist.Add(DbInsert);
                SelectList.Add(I_Sql.GetSqliteInsert());
            }

            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            foreach (DataRow dr in SqlTable.Rows)
            {
                int id = Int32.Parse(dr["ID"].ToString());
                listsql.Add(id);
            }
            foreach (DataRow dr in SqliteTable.Rows)
            {
                int id = Int32.Parse(dr["ID"].ToString());
                listsqlite.Add(id);
            }
            difference = listsqlite.Except(listsql).ToList();
            foreach (int Select in difference)
            {
                Db_Sqlite.ExSQL(((SqlInterface)I_Sql).GetSqliteDelect(Select));                         //删除Service端没有的数据
            }

            int m = Db_Sqlite.ReExNum(SelectList, parlist);
            WriteLog("公司表需要更新" + SelectList.Count().ToString() + "条记录" + "  实际更新" + m.ToString() + "条记录");
        }


        private DbParameter[] ExecutionSelectCompany(CompanyModel DataPar)
        {
            DbParameter[] sqliteparList = new DbParameter[9];
            sqliteparList[0] = Db_Sqlite.CreateDbParameter("@UpdateTime", DataPar.CompnayUpdateTime);
            sqliteparList[1] = Db_Sqlite.CreateDbParameter("@Id", DataPar.CompanyID);
            sqliteparList[2] = Db_Sqlite.CreateDbParameter("@CompanyName_EN", DataPar.CompanyName_EN);
            sqliteparList[3] = Db_Sqlite.CreateDbParameter("@RoomNum", DataPar.CompanyRoomNum);
            sqliteparList[4] = Db_Sqlite.CreateDbParameter("@CompanyName_CN", DataPar.CompanyName_CN);
            sqliteparList[5] = Db_Sqlite.CreateDbParameter("@Floor", DataPar.CompanyFloor);
            sqliteparList[6] = Db_Sqlite.CreateDbParameter("@Introduction", DataPar.CompanyContent);
            sqliteparList[7] = Db_Sqlite.CreateDbParameter("@Building", DataPar.CompanyBuild);
            sqliteparList[8] = Db_Sqlite.CreateDbParameter("@SearchLetter", DataPar.CompanySearch);
            return sqliteparList;

        }


        private Dictionary<int, CompanyModel> GetSqlcCompany(DataTable table)
        {
            Dictionary<int, CompanyModel> dic = new Dictionary<int, CompanyModel>();
            foreach (DataRow dr in table.Rows)
            {
                CompanyModel compnaymodel = new CompanyModel();
                compnaymodel.CompanyName_CN = dr["CompanyName"].ToString();
                compnaymodel.CompanyName_EN = dr["EnglishName"].ToString();
                compnaymodel.CompanyID = Int32.Parse(dr["ID"].ToString());
                compnaymodel.CompanyFloor = dr["FloorNum"].ToString();
                compnaymodel.CompanyContent = dr["CompanyInfo"].ToString();
                compnaymodel.CompanyBuild = dr["BeamNum"].ToString();
                compnaymodel.CompanySearch = dr["Pingying"].ToString();
                compnaymodel.CompanyRoomNum = dr["RoomNum"].ToString();
                compnaymodel.CompnayUpdateTime = ((DateTime)dr["Update_Time"]).ToString("yyyy/MM/dd HH:mm:ss.fff ");
                dic.Add(Int32.Parse(dr["Id"].ToString()), compnaymodel);
            }
            return dic;
        }
        #endregion

        #region 通知表数据同步

        private void SynchDataNotice()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Notice);
            SqlTable = Db_Sqlite.ReSelectdtb(((SqlInterface)I_Sql).GetLatestUpdateTime());
            if (SqlTable.Rows.Count > 0)
            {
                updatetime = SqlTable.Rows[0]["UpdateTime"].ToString();
            }
            NoticeData(updatetime);
        }

        private void NoticeData(string updatetime)
        {
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Notice);
            DataTable SqlTable = new DataTable();
            DataTable SqliteTable = new DataTable();
            List<int> listsql = new List<int>();
            List<int> listsqlite = new List<int>();
            List<int> difference = new List<int>();
            List<string> SelectList = new List<string>();
            SqlTable = Db_Sql.ReSelectdtb(((SqlInterface)I_Sql).GetDatafromSql(updatetime));
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            Dictionary<int, NoticeModel> sqltablelist = GetSqlcNotice(SqlTable);
            IList<DbParameter[]> parlist = new List<DbParameter[]>();

            for (int j = 0; j < SqliteTable.Rows.Count; j++)
            {
                int id = Int32.Parse(SqliteTable.Rows[j]["ID"].ToString());
                for (int i = 0; i < sqltablelist.Count; i++)
                {
                    if (id == sqltablelist.Keys.ToList()[i])
                    {
                        DbParameter[] DbUpdate = ExecutionSelectNotice(sqltablelist[id]);
                        parlist.Add(DbUpdate);
                        SelectList.Add(I_Sql.GetSqliteUpdate());
                        sqltablelist.Remove(id);
                        i--;
                    }
                }
            }

            foreach (int Select in sqltablelist.Keys)                                               //根据差集进行插入操作
            {
                DbParameter[] DbInsert = ExecutionSelectNotice(sqltablelist[Select]);
                parlist.Add(DbInsert);
                SelectList.Add(I_Sql.GetSqliteInsert());
            }

            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            foreach (DataRow dr in SqlTable.Rows)
            {
                int id = Int32.Parse(dr["ID"].ToString());
                listsql.Add(id);
            }
            foreach (DataRow dr in SqliteTable.Rows)
            {
                int id = Int32.Parse(dr["ID"].ToString());
                listsqlite.Add(id);
            }
            difference = listsqlite.Except(listsql).ToList();
            foreach (int Select in difference)
            {
                Db_Sqlite.ExSQL(((SqlInterface)I_Sql).GetSqliteDelect(Select));                         //删除Service端没有的数据
            }

            int m = Db_Sqlite.ReExNum(SelectList, parlist);
            WriteLog("通知表需要更新" + SelectList.Count().ToString() + "条记录" + "  实际更新" + m.ToString() + "条记录");
        }


        private DbParameter[] ExecutionSelectNotice(NoticeModel DataPar)
        {
            string WebSit = WebPath + DataPar.NoticePath;
            DataPar.NoticePath = DataPar.NoticePath.Substring(DataPar.NoticePath.LastIndexOf('/') + 1);
            DbParameter[] sqliteparList = new DbParameter[13];
            sqliteparList[0] = Db_Sqlite.CreateDbParameter("@UpdateTime", DataPar.NoticeUpdateTime);
            sqliteparList[1] = Db_Sqlite.CreateDbParameter("@Id", DataPar.NoticeID);
            sqliteparList[2] = Db_Sqlite.CreateDbParameter("@Author", DataPar.NoticeAuthor);
            sqliteparList[3] = Db_Sqlite.CreateDbParameter("@Content", DataPar.NoticeContent);
            sqliteparList[4] = Db_Sqlite.CreateDbParameter("@EndTime", DataPar.NoticeEndTime);
            sqliteparList[5] = Db_Sqlite.CreateDbParameter("@FullScreen", DataPar.NoticeFullScreen);
            sqliteparList[6] = Db_Sqlite.CreateDbParameter("@Path", DataPar.NoticePath);
            sqliteparList[7] = Db_Sqlite.CreateDbParameter("@StartTime", DataPar.NoticeStarTime);
            sqliteparList[8] = Db_Sqlite.CreateDbParameter("@NoticeTime", DataPar.NoticeTime);
            sqliteparList[9] = Db_Sqlite.CreateDbParameter("@Title", DataPar.NoticeTitle);
            sqliteparList[10] = Db_Sqlite.CreateDbParameter("@Type", DataPar.NoticeType);
            sqliteparList[11] = Db_Sqlite.CreateDbParameter("@Used", 1);
            sqliteparList[12] = Db_Sqlite.CreateDbParameter("@Uri", DataPar.NoticeUri);
            if (DataPar.NoticePath != null && DataPar.NoticePath != string.Empty)
            {
                LoadSavePath(DataPar, WebSit);
            }
            return sqliteparList;
        }

        private void LoadSavePath(NoticeModel model, string WebSit)
        {
            string SavePath = string.Empty;
            SavePath = model.NoticePath;
            switch (model.NoticeType)
            {
                case "2":
                    SavePath = LoadPath + @"\Image\" + SavePath;
                    DownLoad(WebSit, SavePath);
                    break;
                case "3":
                    SavePath = LoadPath + @"\Video\" + SavePath;
                    DownLoad(WebSit, SavePath);
                    break;
                case "4":
                    SavePath = LoadPath + @"\Flash\" + SavePath;
                    DownLoad(WebSit, SavePath);
                    break;
                default:
                    break;
            }
        }


        private Dictionary<int, NoticeModel> GetSqlcNotice(DataTable table)
        {
            Dictionary<int, NoticeModel> dic = new Dictionary<int, NoticeModel>();
            foreach (DataRow dr in table.Rows)
            {
                NoticeModel noticemodel = new NoticeModel();
                noticemodel.NoticeTitle = dr["Title"].ToString();
                noticemodel.NoticeAuthor = dr["Author"].ToString();
                noticemodel.NoticeID = Int32.Parse(dr["ID"].ToString());
                noticemodel.NoticeContent = dr["Content"].ToString();
                noticemodel.NoticeUpdateTime = ((DateTime)dr["Update_Time"]).ToString("yyyy/MM/dd HH:mm:ss.fff ");
                noticemodel.NoticeTime = dr["NoticeTime"].ToString();
                noticemodel.NoticeType = dr["FileType"].ToString();
                bool judge = bool.Parse(dr["IsFull"].ToString());
                if (judge)
                {
                    noticemodel.NoticeFullScreen = "1";
                }
                else
                {
                    noticemodel.NoticeFullScreen = "0";
                }
                noticemodel.NoticeStarTime = dr["StartTime"].ToString();
                noticemodel.NoticeEndTime = dr["Endtime"].ToString();
                noticemodel.NoticePath = dr["FilePath"].ToString();
                noticemodel.NoticeUri = dr["FileName"].ToString();

                dic.Add(Int32.Parse(dr["ID"].ToString()), noticemodel);
            }
            return dic;
        }

        #endregion

        #region 物业概况表同步数据

        private void SynchDataRule()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            DataTable SqliteTable = new DataTable();
            List<string> SelectList = new List<string>();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Rule);
            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            IList<DbParameter[]> parlist = new List<DbParameter[]>();
            RuleModel rulemodel = GetRuleModel(SqlTable);
            if ((SqliteTable.Rows.Count > 0) && (SqlTable.Rows.Count > 0) && SqliteTable != null && SqlTable != null)
            {
                if (DateTime.Parse(SqlTable.Rows[0]["UpdateTime"].ToString()) > DateTime.Parse(SqliteTable.Rows[0]["UpdateTime"].ToString()))
                {
                    if (rulemodel == null) return;
                    DbParameter[] DbInsert = ExecutionSelectRule(rulemodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteUpdate());
                }
            }
            else
            {
                if (rulemodel != null)
                {
                    DbParameter[] DbInsert = ExecutionSelectRule(rulemodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteInsert());
                }
            }

            int m = Db_Sqlite.ReExNum(SelectList, parlist);
            WriteLog("物业概况表需要更新" + SelectList.Count().ToString() + "条记录" + "  实际更新" + m.ToString() + "条记录");
        }

        private RuleModel GetRuleModel(DataTable table)
        {
            RuleModel rulemodel = new RuleModel();
            if (table == null || table.Rows.Count == 0) return null;
            rulemodel.RuleAddress = table.Rows[0]["Address"].ToString();
            rulemodel.RuleTelephone = table.Rows[0]["Phone"].ToString();
            rulemodel.RuleInternet = table.Rows[0]["WebSit"].ToString();
            rulemodel.RuleID = Int32.Parse(table.Rows[0]["Id"].ToString());
            rulemodel.RuleContent = table.Rows[0]["Content"].ToString();
            rulemodel.RuleBus = table.Rows[0]["BusRoute"].ToString();
            rulemodel.UpdateTime = ((DateTime)table.Rows[0]["UpdateTime"]).ToString("yyyy/MM/dd HH:mm:ss.fff ");
            rulemodel.RuleImage = table.Rows[0]["FilePath"].ToString();
            return rulemodel;
        }

        private DbParameter[] ExecutionSelectRule(RuleModel DataPar)
        {
            string WebSit = WebPath + DataPar.RuleImage;
            DataPar.RuleImage = DataPar.RuleImage.Substring(DataPar.RuleImage.LastIndexOf('/') + 1);
            DbParameter[] sqliteparList = new DbParameter[8];
            sqliteparList[0] = Db_Sqlite.CreateDbParameter("@UpdateTime", DataPar.UpdateTime);
            sqliteparList[1] = Db_Sqlite.CreateDbParameter("@ID", DataPar.RuleID);
            sqliteparList[2] = Db_Sqlite.CreateDbParameter("@Address", DataPar.RuleAddress);
            sqliteparList[3] = Db_Sqlite.CreateDbParameter("@Telephone", DataPar.RuleTelephone);
            sqliteparList[4] = Db_Sqlite.CreateDbParameter("@Internet", DataPar.RuleInternet);
            sqliteparList[5] = Db_Sqlite.CreateDbParameter("@Content", DataPar.RuleContent);
            sqliteparList[6] = Db_Sqlite.CreateDbParameter("@Bus", DataPar.RuleBus);
            sqliteparList[7] = Db_Sqlite.CreateDbParameter("@Image", DataPar.RuleImage);
            string SavePath = LoadPath + @"\Resources\" + DataPar.RuleImage;
            if (DataPar.RuleImage != null && DataPar.RuleImage != string.Empty)
            {
                try
                {
                    DownLoad(WebSit, SavePath);
                }
                catch (Exception ex)
                {
                    WriteLog("没有物业概况图片资源" + ex.Message.ToString());
                }

            }
            return sqliteparList;
        }


        #endregion

        #region 时间设置表同步数据

        private void SynchDataSeting()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            DataTable SqliteTable = new DataTable();
            List<string> SelectList = new List<string>();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Seting);
            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            IList<DbParameter[]> parlist = new List<DbParameter[]>();
            SetModel setmodel = GetSetModel(SqlTable);
            if ((SqliteTable.Rows.Count > 0) && (SqlTable.Rows.Count > 0) && SqliteTable != null && SqlTable != null)
            {
                if (DateTime.Parse(SqlTable.Rows[0]["UpdateTime"].ToString()) > DateTime.Parse(SqliteTable.Rows[0]["UpdateTime"].ToString()))
                {
                    if (setmodel == null) return;
                    DbParameter[] DbInsert = ExecutionSelectSet(setmodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteUpdate());
                }
            }
            else
            {
                if (setmodel != null)
                {
                    DbParameter[] DbInsert = ExecutionSelectSet(setmodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteInsert());
                }
            }

            int m = Db_Sqlite.ReExNum(SelectList, parlist);
            WriteLog("参数配置表需要更新" + SelectList.Count().ToString() + "条记录" + "  实际更新" + m.ToString() + "条记录");
        }

        private SetModel GetSetModel(DataTable table)
        {
            SetModel setmodel = new SetModel();
            if (table == null || table.Rows.Count == 0) return null;
            setmodel.SetID = Int32.Parse(table.Rows[0]["Id"].ToString());
            setmodel.SetIdelTime = table.Rows[0]["SysIdleTime"].ToString();
            setmodel.SetNoticeShowTime = table.Rows[0]["PostShowTime"].ToString();
            setmodel.SetUpdateTime = ((DateTime)table.Rows[0]["Updatetime"]).ToString("yyyy/MM/dd HH:mm:ss.fff ");
            WebPath = table.Rows[0]["URL"].ToString();
            return setmodel;
        }

        private DbParameter[] ExecutionSelectSet(SetModel DataPar)
        {
            DbParameter[] sqliteparList = new DbParameter[4];
            sqliteparList[0] = Db_Sqlite.CreateDbParameter("@UpdateTime", DataPar.SetUpdateTime);
            sqliteparList[1] = Db_Sqlite.CreateDbParameter("@Id", DataPar.SetID);
            sqliteparList[2] = Db_Sqlite.CreateDbParameter("@IdleTime", DataPar.SetIdelTime);
            sqliteparList[3] = Db_Sqlite.CreateDbParameter("@NoticeShowTime", DataPar.SetNoticeShowTime);
            return sqliteparList;

        }

        #endregion

        #region 服务表同步数据

        private void SynchDataService()
        {
            string updatetime = string.Empty;
            DataTable SqlTable = new DataTable();
            DataTable SqliteTable = new DataTable();
            List<string> SelectList = new List<string>();
            I_Sql = SqlFactory.MakeSelect(TableType.Table_Type.Service);
            SqlTable = Db_Sql.ReSelectdtb(I_Sql.GetSqlSelect());
            SqliteTable = Db_Sqlite.ReSelectdtb(I_Sql.GetSqliteSelect());
            IList<DbParameter[]> parlist = new List<DbParameter[]>();
            ServiceModel servicemodel = GetServiceModel(SqlTable);
            if ((SqliteTable.Rows.Count > 0) && (SqlTable.Rows.Count > 0) && SqliteTable != null && SqlTable != null)
            {
                if (DateTime.Parse(SqlTable.Rows[0]["Update_Time"].ToString()) > DateTime.Parse(SqliteTable.Rows[0]["UpdateTime"].ToString()))
                {
                    if (servicemodel == null) return;
                    DbParameter[] DbInsert = ExecutionSelectSet(servicemodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteUpdate());
                }
            }
            else
            {
                if (servicemodel != null)
                {
                    DbParameter[] DbInsert = ExecutionSelectSet(servicemodel);
                    parlist.Add(DbInsert);
                    SelectList.Add(I_Sql.GetSqliteInsert());
                }

            }

            int m = Db_Sqlite.ReExNum(SelectList, parlist);
            WriteLog("服务表需要更新" + SelectList.Count().ToString() + "条记录" + "  实际更新" + m.ToString() + "条记录");
        }

        private ServiceModel GetServiceModel(DataTable table)
        {
            ServiceModel setmodel = new ServiceModel();
            if (table == null || table.Rows.Count == 0) return null;
            setmodel.ServiceID = Int32.Parse(table.Rows[0]["Id"].ToString());
            setmodel.ServiceSale = table.Rows[0]["SZservice"].ToString();
            setmodel.ServiceContent = table.Rows[0]["Content"].ToString();
            setmodel.ServiceAfterSale = table.Rows[0]["SHservice"].ToString();
            setmodel.SetUpdateTime = ((DateTime)table.Rows[0]["Update_Time"]).ToString("yyyy/MM/dd HH:mm:ss.fff ");
            return setmodel;
        }

        private DbParameter[] ExecutionSelectSet(ServiceModel DataPar)
        {
            DbParameter[] sqliteparList = new DbParameter[5];
            sqliteparList[0] = Db_Sqlite.CreateDbParameter("@UpdateTime", DataPar.SetUpdateTime);
            sqliteparList[1] = Db_Sqlite.CreateDbParameter("@Id", DataPar.ServiceID);
            sqliteparList[2] = Db_Sqlite.CreateDbParameter("@Sale", DataPar.ServiceSale);
            sqliteparList[3] = Db_Sqlite.CreateDbParameter("@AfterSale", DataPar.ServiceAfterSale);
            sqliteparList[4] = Db_Sqlite.CreateDbParameter("@Content", DataPar.ServiceContent);
            return sqliteparList;

        }

        #endregion

        /// <summary>
        /// 创建相应的文件和文件夹
        /// </summary>
        private string CreateFile()
        {
            string path = this.BrowseName.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                //WriteLog("请选择文件夹");
                return "";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + @"\UpdateFile";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string SouPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data.db");
            string DisPath = path + @"\Data.db";
            if (!Directory.Exists(DisPath))
            {
                System.IO.File.Copy(SouPath, DisPath, true);
            }
            string Connect = @"Data Source=" + DisPath + @";Initial Catalog=T_Company;Persist Security Info=True;";
            Db_Sqlite = new DbHelper(Connect, DbProvider.Sqlite);//连接sqlite数据库

            return path;
        }

        /// <summary>
        /// 从网上下载图片寸到指定的文件夹中去
        /// </summary>
        private bool DownLoad(string Source, string Destination)
        {
            bool result = false;
            try
            {
                WebRequest request = WebRequest.Create(Source);
                WebResponse response = request.GetResponse();                                 //连接网页图片
                FileInfo fi = new FileInfo(Destination);
                string dir = fi.Directory.FullName;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                FileStream writer = new FileStream(Destination, FileMode.OpenOrCreate, FileAccess.Write);
                Stream reader = response.GetResponseStream();
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();

                reader.Close();
                reader.Dispose();
                response.Close();
                result = true;
            }
            catch (Exception ex)
            {
                WriteLog("下载失败，文件路径为:" + Source + "目标路径为:" + Destination + @"错误:" + ex.Message.ToString());
            }
            return result;
        }

        private void WriteLog(string info)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      this.logText.Text = this.logText.Text + "\n" + info;
                  }));
        }
    }
}
