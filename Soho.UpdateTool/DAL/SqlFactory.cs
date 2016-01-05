using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.DAL
{
    public class CompanySql : SqlInterface
    {
        public string GetSqlSelect()
        {
            return @"SELECT  [Id]
  FROM [dbo].[dt_Company] WHERE DeviceId='"+Device.Name+"'";
        }

        public string GetSqliteSelect()
        {
            return @"SELECT ID FROM T_Company";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_Company set [CompanyName_EN]=@CompanyName_EN,[ID]=@Id,[RoomNum]=@RoomNum,[CompanyName_CN]=@CompanyName_CN,[Floor]=@Floor,
                                [Introduction]=@Introduction,[Building]=@Building,[SearchLetter]=@SearchLetter,[UpdateTime]=@UpdateTime where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_Company ([CompanyName_EN],[ID],[RoomNum],[CompanyName_CN],[Floor],[Introduction],[Building],[SearchLetter],[UpdateTime]) VALUES(
                                @CompanyName_EN,@Id,@RoomNum,@CompanyName_CN,@Floor,@Introduction,@Building,@SearchLetter,@UpdateTime)";
        }

        public string GetSqliteDelect(int id)
        {
            return @"DELETE FROM T_Company WHERE Id=" + id;
        }

        public string GetLatestUpdateTime()
        {
            return @"SELECT ID,UpdateTime FROM T_Company ORDER BY UpdateTime DESC";
        }

        public string GetDatafromSql(string updatetime)
        {
            return @"SELECT ID
      ,[CompanyName]
      ,[RoomNum]
      ,[PhoneNum]
      ,[CompanyInfo]
      ,[ImagePath]
      ,[IconPath]
      ,[FloorNum]
      ,[Link_Url]
      ,[BeamNum]
      ,[Update_Time]
      ,[DeviceId]
      ,[EnglishName]
      ,[CustomerId]
      ,[Pingying]
FROM [dbo].[dt_Company] WHERE Update_Time>'" + updatetime + "' AND DeviceId='"+Device.Name+"'";
        }

    }

    public class MenuSql : SqlInterface
    {
        public string GetSqlSelect()
        {
            return @"SELECT [Id]
  FROM [dbo].[B_MainPageInfo] WHERE DeviceId='"+Device.Name+"'";
        }

        public string GetSqliteSelect()
        {
            return @"SELECT ID FROM T_Menu";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_Menu set [ClassName]=@ClassName,[Icon]=@Icon,[UserControlIndex]=@UserControlIndex,[UpdateTime]=@UpdateTime where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_Menu ([ClassName],[ID],[Icon],[UserControlIndex],[UpdateTime]) VALUES(
                                @ClassName,@Id,@Icon,@UserControlIndex,@UpdateTime)";
        }

        public string GetSqliteDelect(int id)
        {
            return @"DELETE FROM T_Menu WHERE Id=" + id;
        }

        public string GetLatestUpdateTime()
        {
            return @"SELECT ID,UpdateTime FROM T_Menu ORDER BY UpdateTime DESC";
        }

        public string GetDatafromSql(string updatetime)
        {
            return @"SELECT ID
      ,[LconPath]
      ,[IndexNo]
      ,[Introduction]
      ,[PagePath]
      ,[AssemblyPath]
      ,[DeviceId]
      ,[AddTime]
      ,[Updatetime]
FROM [dbo].[B_MainPageInfo] WHERE UpdateTime>'" + updatetime + "' AND  DeviceId='" + Device.Name + "'";
        }
    }

    public class NoticeSql : SqlInterface
    {
        public string GetSqlSelect()
        {
            return @"SELECT [Id]
          
  FROM [dbo].[dt_Notice] WHERE DeviceId='" + Device.Name + "'";
        }

        public string GetSqliteSelect()
        {
            return @"SELECT ID FROM T_Notice";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_Notice set [Title]=@Title,[ID]=@Id,[Content]=@Content,[NoticeTime]=@NoticeTime,[Author]=@Author,[Path]=@Path,[Used]=@Used,
                    [Type]=@Type,[StartTime]=@StartTime,[FullScreen]=@FullScreen,[EndTime]=@EndTime,[UpdateTime]=@UpdateTime,[Uri]=Uri where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_Notice ([Title],[Content],[ID],[NoticeTime],[Author],[Path],[Used],[Type],[StartTime],[FullScreen],[EndTime],[UpdateTime],[Uri]) VALUES(
                                @Title,@Content,@Id,@NoticeTime,@Author,@Path,@Used,@Type,@StartTime,@FullScreen,@EndTime,@UpdateTime,@Uri)";
        }

        public string GetSqliteDelect(int id)
        {
            return @"DELETE FROM T_Notice WHERE Id=" + id;
        }

        public string GetLatestUpdateTime()
        {
            return @"SELECT ID,UpdateTime FROM T_Notice ORDER BY UpdateTime DESC";
        }

        public string GetDatafromSql(string updatetime)
        {
            return @"SELECT ID
      ,[Title]
      ,[Author]
      ,[Content]
      ,[Update_Time]
      ,[DeviceId]
      ,[CustomerId]
      ,[NoticeTime]
      ,[FileType]
      ,[IsFull]
      ,[Flag]
      ,[StartTime]
      ,[Endtime]
      ,[FilePath]
      ,[FileName]
      ,[FormJson]
FROM [dbo].[dt_Notice] WHERE Update_Time>'" + updatetime + "' AND DeviceId='" + Device.Name + "'";
        }
    }

    public class RuleSql : ISqlUpdate
    {
        public string GetSqliteSelect()
        {
            return @"SELECT ID,UpdateTime FROM T_About";
        }
        public string GetSqlSelect()
        {
            return @"SELECT [Id]
      ,[Content]
      ,[Address]
      ,[Phone]
      ,[FilePath]
      ,[WebSit]
      ,[BusRoute]
      ,[UpdateTime]
  FROM [dbo].[B_PropertyIntroduce] WHERE DeviceId='" + Device.Name + "'";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_About set [Content]=@Content,[Telephone]=@Telephone,[Internet]=@Internet,[Address]=@Address,[UpdateTime]=@UpdateTime,[Bus]=@Bus,[Image]=@Image where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_About ([Content],[Telephone],[Internet],[Address],[UpdateTime],[Bus],[ID],[Image]) VALUES(
                                @Content,@Telephone,@Internet,@Address,@UpdateTime,@Bus,@Id,@Image)";
        }
    }

    public class SetSql : ISqlUpdate
    {
        public string GetSqliteSelect()
        {
            return @"SELECT ID,UpdateTime FROM T_ConfigTime";
        }
        public string GetSqlSelect()
        {
            return @"SELECT [Id]
      ,[SysIdleTime]
      ,[PostShowTime] 
      ,[Updatetime]
      ,[URL]
  FROM [dbo].[B_SysParameter] WHERE DeviceId='" + Device.Name + "'";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_ConfigTime set [IdleTime]=@IdleTime,[NoticeShowTime]=@NoticeShowTime,[UpdateTime]=@UpdateTime where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_ConfigTime ([IdleTime],[NoticeShowTime],[UpdateTime],[ID]) VALUES(
                                @IdleTime,@NoticeShowTime,@UpdateTime,@ID)";
        }

    }

    public class ServiceSql : ISqlUpdate
    {
        public string GetSqliteSelect()
        {
            return @"SELECT ID,UpdateTime FROM T_Service";
        }
        public string GetSqlSelect()
        {
            return @"  SELECT [Id]
      ,[SHservice]
      ,[SZservice]
      ,[Content]
      ,[Update_Time]
  FROM [dbo].[dt_Service] WHERE DeviceId='" + Device.Name + "'";
        }

        public string GetSqliteUpdate()
        {
            return @"UPDATE  T_Service set [Sale]=@Sale,[AfterSale]=@AfterSale,[Content]=@Content,[UpdateTime]=@UpdateTime where ID=@Id";
        }

        public string GetSqliteInsert()
        {
            return @"INSERT INTO T_Service ([Sale],[AfterSale],[Content],[UpdateTime],[ID]) VALUES(
                                @Sale,@AfterSale,@Content,@UpdateTime,@Id)";
        }

    }




    /************************************************************************/
    /* Add by Ron.shen 强制下载对象SQL语句
    /************************************************************************/
    public class DownFileSql : ISqlUpdate
    {
        public string GetSqliteSelect()
        {
            return "";
        }
        public string GetSqlSelect()
        {
            return @"select Id,URL,Path,UpdateTime from ResourceInfo where IsDel=1 and IsStart=1 and IsDownLoad=0 and DeviceId='" + Device.Name + "'";
        }

        /************************************************************************/
        /* 强制下载对象需要更新数据库中是否已被下载的字段，而接口中并没有定义对应的SQLUpdate方法，为了省去修改接口以后各            继承的类都需要修改的情况，所以此处虽然采用接口中定义的GetSqliteUpdate方法，但是实际上是实现了更新SQL中字段的            语句                                                                 */
        /************************************************************************/
        public string GetSqliteUpdate()
        {
            return "UPDATE ResourceInfo  SET  IsDownLoad = 1 ,[UpdateTime] = getdate() WHERE Id=@ID";
        }

        public string GetSqliteInsert()
        {
            return "";
        }
    }




    class Device
    {
        static string _Name = "201312191428592859158";
        public static string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}
