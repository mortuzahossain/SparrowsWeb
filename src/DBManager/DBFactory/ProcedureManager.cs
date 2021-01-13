using DBManager.OracleManager;
using DBManager.SQLManager;
using DBManager.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static GlobalEntities.Enums.GlobalEnums;
using GlobalEntities.Variables;
using GlobalEntities.Entities;

namespace DBManager
{
    public class ProcedureManager
    {
        SqlProcedureManager objSqlProcedureManager;
        OracleProcedureManager objOracleProcedureManager;

        DBConnection dbConnection;
        private DBProvider dbProvider;
        private string dbConStr;
        private string dbWHName;

      

        //public ProcedureManager(DBName dbName)
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
        //            objOracleProcedureManager = new OracleProcedureManager(dbConStr);
        //        else
        //            objSqlProcedureManager = new SqlProcedureManager(dbConStr);
        //    }
        //}
        public ProcedureManager(DBName dbName, ref string resp)
        {
            dbProvider = CommonVariables.GetDbProvider();
            dbWHName = CommonVariables.GetWHName();

            if (string.IsNullOrEmpty(dbConStr))
            {
                if (GetDBCon(dbName,ref resp))
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
                    objOracleProcedureManager = new OracleProcedureManager(dbConStr);
                else
                    objSqlProcedureManager = new SqlProcedureManager(dbConStr);
            }
        }
        private bool GetDBCon(DBName dbName, ref string respmsg)
        {
            dbConnection = new DBConnection(dbName.ToString(), dbWHName, dbProvider.ToString());
            bool flag = ConnectionManager.GetConnection(ref dbConnection);
            dbConStr = dbConnection.ConnectionString;
            respmsg = dbConnection.RespMsg;
            return flag;
        }

        #region Insert

        public Int32 ExecuteNonQuery(string SPName, ref string reply, params object[] parameterValues)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    return objOracleProcedureManager.ExecuteNonQuery(SPName, ref reply, parameterValues);
                    //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue);
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlProcedureManager.ExecuteNonQuery(SPName, ref reply, parameterValues);
                }
            }
            catch (Exception errorExcep)
            {
                reply = reply + ". " + errorExcep.Message;
                return 0;
            }

            return 0;
        }
        #endregion
        public DataTable ExecSPreturnDataTable(string SPName, ref string reply, params object[] parameterValues)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    return objOracleProcedureManager.ExecuteDataTable(SPName, ref reply, parameterValues);
                    //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue);
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlProcedureManager.ExecuteDataTable(SPName, ref reply, parameterValues);
                }
            }
            catch (Exception errorExcep)
            {
                reply = errorExcep.Message + ". Inner Message " + reply;
                return null;
            }

            return null;
        }
        public IDataReader ExecSPreturnIDataReader(string SPName, ref string reply, params object[] parameterValues)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    IDataReader reader = objOracleProcedureManager.ExecuteReader(SPName, ref reply, parameterValues);
                    return reader;
                    //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue);
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    IDataReader reader = objSqlProcedureManager.ExecuteReader(SPName, ref reply, parameterValues);
                    return reader; 
                }
            }
            catch 
            {
                return null;
            }

            return null;
        }

        //public IDataReader ExecSPreturnIDataReader(string SPName, ref string reply, params object[] parameterValues)
        //{
        //    try
        //    {
        //        if (dbProvider == DBProvider.Oracle)
        //        {
        //            DataTableReader reader = objOracleProcedureManager.ExecuteDataTable(SPName, ref reply, parameterValues).CreateDataReader();
        //            return reader;
        //            //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue);
        //        }
        //        else if (dbProvider == DBProvider.SqlClient)
        //        {
        //            DataTableReader reader = objSqlProcedureManager.ExecuteDataTable(SPName, ref reply, parameterValues).CreateDataReader();
        //            return reader;
        //        }
        //    }
        //    catch (Exception errorExcep)
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        public String ExecSPreturnString(string SPName, ref string reply, params object[] FieldValue)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    return objOracleProcedureManager.ExecuteDataTable(SPName, ref reply, FieldValue).Rows[0][0].ToString();
                    //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue).Rows[0][0].ToString();
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlProcedureManager.ExecuteDataTable(SPName, ref reply, FieldValue).Rows[0][0].ToString();
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
        public DataSet ReturnDataSet(string sqlstring)
        {
            //try
            //{
            //    return objSqlQueryManager.ReturnDataSet(sqlstring);
            //}
            //catch (Exception errorExcep)
            //{
            return null;
            //
            //return null;
        }
        public string InsertRequest(string AppId, string DeviceIP, string MTI, string MText, string RefNo, ref string errMsg)
        {
            try
            {
                objSqlProcedureManager.ExecuteInsertRequest(AppId, DeviceIP, MTI, MText, RefNo, ref errMsg);

            }
            catch (Exception Err)
            {
                errMsg = "Error:" + Err.Message;
                //throw new Exception(Err.Message);
            }
            return "0";

        }

        public List<SPParameter> GetSPParameters(string SPName, ref string reply)
        {
            try
            {
                if (dbProvider == DBProvider.Oracle)
                {
                    return objOracleProcedureManager.GetSPParameters(SPName, ref reply);
                    //return objOracleProcedureManager.ExecuteDataTableTest(SPName, ref reply, FieldValue);
                }
                else if (dbProvider == DBProvider.SqlClient)
                {
                    return objSqlProcedureManager.GetSPParameters(SPName, ref reply);
                }
            }
            catch (Exception errorExcep)
            {
                reply = errorExcep.Message + ". Inner Message " + reply;
                return null;
            }

            return null;
        }


    }
}
