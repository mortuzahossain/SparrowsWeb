using DBManager.Common;
using DBManager.OracleManager;
using DBManager.SQLManager;
using GlobalEntities.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static GlobalEntities.Enums.GlobalEnums;

namespace DBManager
{
   public class QueryManager
    {

        SqlQueryManager objSqlQueryManager;
        OracleQueryManager objOracleQueryManager;

        DBConnection dbConnection;
        private DBProvider dbProvider;
        private string dbConStr;
        private string dbWHName;
        //public QueryManager(DBName dbName)
        //{
        //    dbProvider = CommonVariables.GetDbProvider();
        //    dbWHName = CommonVariables.GetWHName();

        //    if (string.IsNullOrEmpty(dbConStr))
        //    {
        //        if (GetDBCon(dbName))
        //        {
        //            //resp = "Connection Found";
        //        }
        //        else
        //        {
        //            //resp = "Connection not found, Please configure the database connection.";
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(dbConStr))
        //    {
        //        if (CommonVariables.GetDbProvider() == DBProvider.Oracle)
        //            objOracleQueryManager = new OracleQueryManager(dbConStr);
        //        else
        //            objSqlQueryManager = new SqlQueryManager(dbConStr);
        //    }
        //}
        public QueryManager(DBName dbName, ref string resp)
        {
            dbProvider = CommonVariables.GetDbProvider();
            dbWHName = CommonVariables.GetWHName();

            if (string.IsNullOrEmpty(dbConStr))
            {
                if (GetDBCon(dbName, ref resp))
                {
                    resp = "Connection Found";
                }
                else
                {
                    resp = "Connection not found, Please configure the database connection. Details:"+ resp;
                }
            }
            if (!string.IsNullOrEmpty(dbConStr))
            {
                if (CommonVariables.GetDbProvider() == DBProvider.Oracle)
                    objOracleQueryManager = new OracleQueryManager(dbConStr);
                else
                    objSqlQueryManager = new SqlQueryManager(dbConStr);
            }
        }
        public string GetConStr(DBName dbName, ref string respmsg)
        {
            dbConnection = new DBConnection(dbName.ToString(), dbWHName, dbProvider.ToString());
            bool flag = ConnectionManager.GetConnection(ref dbConnection);
            dbConStr = dbConnection.ConnectionString;
            respmsg = dbConnection.RespMsg;
            return dbConStr;
        }
        private bool GetDBCon(DBName dbName, ref string respmsg)
        {
            dbConnection = new DBConnection(dbName.ToString(), dbWHName, dbProvider.ToString());
            bool flag = ConnectionManager.GetConnection(ref dbConnection);
            dbConStr = dbConnection.ConnectionString;
            respmsg = dbConnection.RespMsg;
            return flag;
        }

        public DataSet ExecuteSQLRetDataSet(string SqlStr, ref string reply, params object[] parameterValues)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    SqlStr = SqlStr.Replace(".dbo.", ".");
                    SqlStr = SqlStr.Replace(".DBO.", ".");
                    return objOracleQueryManager.ExecuteDataset(SqlStr, ref reply, parameterValues); 
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlQueryManager.ExecuteDataset(SqlStr, ref reply, parameterValues);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public string ExecuteSingleValue(string SqlStr, ref string reply, params object[] parameterValues)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    SqlStr = SqlStr.Replace(".dbo.", ".");
                    SqlStr = SqlStr.Replace(".DBO.", ".");
                    return objOracleQueryManager.ExecuteSingleValue(SqlStr, ref reply, parameterValues);
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlQueryManager.ExecuteSingleValue(SqlStr, ref reply, parameterValues);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

       

    }
}
