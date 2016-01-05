using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateTool.DAL
{
    class SelectFactory
    {
        public ISqlUpdate MakeSelect(TableType.Table_Type type)
        {
            switch (type)
            {
                case TableType.Table_Type.Compnay:
                    return new CompanySql();
                case TableType.Table_Type.Menu:
                    return new MenuSql();
                case TableType.Table_Type.Notice:
                    return new NoticeSql();
                case TableType.Table_Type.Rule:
                    return new RuleSql();
                case TableType.Table_Type.Seting:
                    return new SetSql();
                case TableType.Table_Type.Service:
                    return new ServiceSql();
                case TableType.Table_Type.DownFile:
                    return new DownFileSql();
                default:
                    return null;
            }
        }
    }
}
