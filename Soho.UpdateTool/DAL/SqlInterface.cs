using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.DAL
{


    public interface ISqlUpdate 
    {
        string GetSqlSelect();
        string GetSqliteSelect();
        string GetSqliteUpdate();
        string GetSqliteInsert();
    }

    public interface SqlInterface : ISqlUpdate
    {
        string GetSqliteDelect(int id);
        string GetLatestUpdateTime();
        string GetDatafromSql(string updatetime);
    }
}
