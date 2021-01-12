using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManager.Common;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Collections;

namespace DBManager.SQLManager
{
    public class SqlQueryManager
    {
        private static string ConStr;
        private SqlConnection SqlCon;
        private SqlDataAdapter adapter;
        private static Hashtable paramCache;
        private enum SqlConnectionOwnership
        {
            Internal = 0,
            External = 1,
        }
        static SqlQueryManager()
        {
            //SqlQueryManager.paramCache = new Hashtable();
        }
        public SqlQueryManager(string ConnStr)
        {
            this.adapter = new SqlDataAdapter();

            SqlQueryManager.ConStr = ConnStr.ToString();
            this.SqlCon = new SqlConnection(ConStr);
        }

        private object IIf(bool Expression, object TruePart, object FalsePart)
        {
            object obj;
            bool bl = !Expression;
            if (!bl)
            {
                obj = TruePart;
            }
            else
            {
                return FalsePart;
            }
            return obj;
        }
        private SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            int num1 = originalParameters.Length - 1;
            SqlParameter[] parameterArray1 = new SqlParameter[num1 + 1];
            int num2 = num1;
            for (int num3 = 0; num3 <= num2; num3++)
            {
                parameterArray1[num3] = (SqlParameter)((ICloneable)originalParameters[num3]).Clone();
            }
            return parameterArray1;

        }
        private SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string queryStr, ref string errMsg, params object[] parameterValues)
        {
            if (connection == null)
            {
                errMsg = string.Empty;
                errMsg = "Error: Connection Error...";
            }
            if ((queryStr == null) || (queryStr.Length == 0))
            {
                errMsg = string.Empty;
                errMsg = "Error:" + new ArgumentNullException("queryStr").Message;
            }
            SqlCommand command1 = new SqlCommand(queryStr, connection);
            command1.CommandType = CommandType.Text;
            connection.Open();
            SqlCommandBuilder.DeriveParameters(command1);
            connection.Close();
            //if (!includeReturnValueParameter)
            //{
            //    command1.Parameters.RemoveAt(0);
            //}
            SqlParameter[] parameterArray1 = new SqlParameter[(command1.Parameters.Count - 1) + 1];
            command1.Parameters.CopyTo(parameterArray1, 0);
            foreach (SqlParameter parameter1 in parameterArray1)
            {
                parameter1.Value = DBNull.Value;
            }
            return parameterArray1;

        }
        private void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues, ref string errMsg)
        {
            try
            {
                if ((commandParameters != null) || (parameterValues != null))
                {
                    //if (commandParameters.Length != parameterValues.Length)
                    //{
                    //    throw new ArgumentException("Parameter count does not match Parameter Value count.");
                    //}
                    int num1 = commandParameters.Length - 1;
                    int num2 = num1;
                    for (int num3 = 0; num3 <= num2; num3++)
                    {
                        if (parameterValues[num3] is IDbDataParameter)
                        {
                            IDbDataParameter parameter1 = (IDbDataParameter)parameterValues[num3];
                            if (parameter1.Value == null)
                            {
                                commandParameters[num3].Value = DBNull.Value;
                            }
                            else
                            {
                                commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameter1.Value);
                            }
                        }
                        else if (parameterValues[num3] == null)
                        {
                            commandParameters[num3].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameterValues[num3]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = string.Empty;
                errMsg = "Error:" + ex.Message;
            }

        }
        private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, System.Data.SqlClient.SqlParameter[] commandParameters, ref bool mustCloseConnection)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if ((commandText == null) || (commandText.Length == 0))
            {
                throw new ArgumentNullException("commandText");
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }

        }
        private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandParameters != null)
            {
                foreach (SqlParameter parameter1 in commandParameters)
                {
                    if (parameter1 != null)
                    {
                        if (((parameter1.Direction == ParameterDirection.InputOutput) || (parameter1.Direction == ParameterDirection.Input)) && (parameter1.Value == null))
                        {
                            parameter1.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter1);
                    }
                }
            }

        }

        private SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, ref string errMsg)
        {
            SqlParameter[] parameterArray1;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            try
            {
                this.SqlCon = (SqlConnection)((ICloneable)connection).Clone();
                errMsg = string.Empty;
                parameterArray1 = this.GetParameter(this.SqlCon, spName, ref errMsg).ToArray();
            }
            finally
            {
                if (this.SqlCon != null)
                {
                    this.SqlCon.Dispose();
                }
            }
            return parameterArray1;

        }
        private List<SqlParameter> GetParameter(SqlConnection connection, string queryStr, ref string errMsg)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("queryStr");
            }
            string text1 =  queryStr; //+ this.IIf(includeReturnValueParameter, ":include ReturnValue Parameter", "").ToString();
            string[] strPar = queryStr.Split('@');
            List<SqlParameter> sqlParamList=new List<SqlParameter>();
            for (int i = 1; i < strPar.Length; i++)
            {
                string[] param = strPar[i].Split(' ');
                SqlParameter sqlparam = new SqlParameter(param[0].ToString(),"");
                sqlParamList.Add(sqlparam); 
            }
            return sqlParamList;
            //SqlParameter[] parameterArray1 = (SqlParameter[])SqlQueryManager.paramCache[text1];

            //if (parameterArray1 == null)
            //{
            //    SqlParameter[] parameterArray2 = this.DiscoverSpParameterSet(connection, queryStr, ref errMsg, new object[0]);
            //    SqlQueryManager.paramCache[text1] = parameterArray2;
            //    parameterArray1 = parameterArray2;
            //}
            //return this.CloneParameters(parameterArray1);

        }
        private SqlParameter[] GetSpParameterSet(string connectionstring, string queryStr, ref string errMsg)
        {
            SqlParameter[] parameterArray1;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.SqlCon = new SqlConnection(connectionstring);
                errMsg = string.Empty;
                parameterArray1 = this.GetParameter(this.SqlCon, queryStr, ref errMsg).ToArray();
            }
            finally
            {
                if (this.SqlCon != null)
                {
                    this.SqlCon.Dispose();
                }
            }
            return parameterArray1;

        }
        
        private int ExecuteNonQuery(SqlConnection connection, string spName, ref string errMsg, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(connection, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, parameterArray1);
            }
            return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, new SqlParameter[0]);
        }

        //
        public SqlParameter[] ExecuteNonQueryParam(string spName, ref string reply, params object[] parameterValues)
        {
            SqlParameter[] param = null;
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, spName, ref reply);
                if (parameterArray1 != null)
                {
                    this.AssignParameterValues(parameterArray1, parameterValues, ref reply);
                    param = this.ExecuteNonQueryParam(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
                }
            }
            return param;
        }
        //
        public SqlParameter[] ExecuteNonQueryParam(SqlConnection connection, string spName, ref string reply, params object[] parameterValues)
        {
            SqlParameter[] param = null;
            SqlConnection OracleCon = null;

            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    OracleCon = (SqlConnection)((ICloneable)connection).Clone();

                    SqlParameter[] parameterArray1 = this.GetSpParameterSet(OracleCon, spName, ref reply);
                    if (parameterArray1 != null)
                    {
                        reply = string.Empty;

                        this.AssignParameterValues(parameterArray1, parameterValues, ref reply);
                        param = this.ExecuteNonQueryParam(OracleCon, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
                    }
                }
            }
            return param;
        }
        private SqlParameter[] ExecuteNonQueryParam(string constr, CommandType commandType, string commandText, ref string reply, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            SqlParameter[] param = null;
            SqlConnection OracleCon = null;

            try
            {
                OracleCon = new SqlConnection(constr);
                OracleCon.Open();

                param = this.ExecuteNonQueryParam(OracleCon, commandType, commandText, ref reply, commandParameters);
                OracleCon.Close();
            }
            catch (Exception ex)
            {
                reply = "Error:" + ex.Message;
            }
            finally
            {
                if (OracleCon != null)
                {
                    if (OracleCon.State == ConnectionState.Open)
                    {
                        OracleCon.Close();
                    }
                }
            }
            return param;

        }
        private SqlParameter[] ExecuteNonQueryParam(SqlConnection connection, CommandType commandType, string commandText, ref string reply, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            SqlParameter[] param = null;
            try
            {
                if (connection == null)
                {
                    throw new ArgumentNullException("connection");
                }
                SqlCommand command1 = new SqlCommand();
                bool flag1 = false;
                this.PrepareCommand(command1, connection, null, commandType, commandText, commandParameters, ref flag1);
                int num1 = command1.ExecuteNonQuery();
                command1.Parameters.Clear();
                //if (flag1)
                //{
                //    connection.Close();
                //}
                param = commandParameters;
                return param;
            }
            catch (Exception ex)
            {
                reply = "Error:" + ex.Message;
                return commandParameters;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        //

        private int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            SqlCommand command1 = new SqlCommand();
            bool flag1 = false;
            this.PrepareCommand(command1, connection, null, commandType, commandText, commandParameters, ref flag1);
            int num1 = command1.ExecuteNonQuery();
            command1.Parameters.Clear();
            if (flag1)
            {
                connection.Close();
            }
            return num1;

        }
        private int ExecuteNonQuery(string connectionstring, CommandType commandType, string commandText, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            int num1;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.SqlCon = new SqlConnection(connectionstring);
                this.SqlCon.Open();
                num1 = this.ExecuteNonQuery(this.SqlCon, commandType, commandText, commandParameters);
            }
            finally
            {
                if (this.SqlCon != null)
                {
                    this.SqlCon.Dispose();
                }
            }
            return num1;

        }
        private int ExecuteNonQuery(string connectionstring, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionstring, commandType, commandText, null);
        }
        public int ExecuteNonQuery(string spName, ref string errMsg, params object[] parameterValues)
        {
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteNonQuery(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1);
            }
            else
            {
                return this.ExecuteNonQuery(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName);
            }

        }
        private SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, System.Data.SqlClient.SqlParameter[] commandParameters, SqlQueryManager.SqlConnectionOwnership connectionOwnership)
        {
            SqlDataReader dataReader;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool flag1 = false;
            SqlCommand command1 = new SqlCommand();
            try
            {
                SqlDataReader reader2;
                this.PrepareCommand(command1, connection, transaction, commandType, commandText, commandParameters, ref flag1);
                if (connectionOwnership == SqlQueryManager.SqlConnectionOwnership.External)
                {
                    reader2 = command1.ExecuteReader();
                }
                else
                {
                    reader2 = command1.ExecuteReader(CommandBehavior.CloseConnection);
                }
                bool flag2 = true;
                foreach (SqlParameter parameter1 in command1.Parameters)
                {
                    if (parameter1.Direction != ParameterDirection.Input)
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    command1.Parameters.Clear();
                }
                dataReader = reader2;
            }
            catch (Exception)
            {
                if (flag1)
                {
                    connection.Close();
                }
                return null;
            }
            return dataReader;

        }
        private SqlDataReader ExecuteReader(string connectionstring, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(connectionstring, commandType, commandText, null);

        }
        private SqlDataReader ExecuteReader(string connectionstring, CommandType commandType, string commandText, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            SqlDataReader dataReader;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.SqlCon = new SqlConnection(connectionstring);
                this.SqlCon.Open();
                dataReader = this.ExecuteReader(this.SqlCon, null, commandType, commandText, commandParameters, SqlQueryManager.SqlConnectionOwnership.Internal);
            }
            catch (Exception)
            {
                if (this.SqlCon != null)
                {
                    this.SqlCon.Dispose();
                }
                return null;
            }
            return dataReader;

        }
        public SqlDataReader ExecuteReader(string spName, ref string errMsg, params object[] parameterValues)
        {
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteReader(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1);
            }
            return this.ExecuteReader(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName);

        }
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionstring, commandType, commandText, null);
        }
        private DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            SqlCommand command1 = new SqlCommand();
            DataSet dataSet = new DataSet();
            bool flag1 = false;
            this.PrepareCommand(command1, connection, null, commandType, commandText, commandParameters, ref flag1);
            try
            {
                this.adapter = new SqlDataAdapter(command1);
                this.adapter.Fill(dataSet);
                command1.Parameters.Clear();
            }
            catch (Exception)
            {
                if (this.adapter != null)
                {
                    this.adapter.Dispose();
                }
            }
            if (flag1)
            {
                connection.Close();
            }
            return dataSet;

        }
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText, params System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            DataSet dataSet;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.SqlCon = new SqlConnection(connectionstring);
                this.SqlCon.Open();
                dataSet = this.ExecuteDataset(this.SqlCon, commandType, commandText, commandParameters);
            }
            finally
            {
                if (this.SqlCon != null)
                {
                    this.SqlCon.Dispose();
                }
            }
            return dataSet;

        }
        public DataSet ExecuteDataset(string queryStr, ref string errMsg, params object[] parameterValues)
        {
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }

            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, queryStr, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.Text, queryStr, parameterArray1);
            }
            return this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.Text, queryStr);

        }
        public string ExecuteSingleValue(string queryStr, ref string errMsg, params object[] parameterValues)
        {
            DataSet ds;
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }

            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, queryStr, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                ds = this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.Text, queryStr, parameterArray1);
                try
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                catch (Exception exp)
                {
                    errMsg = exp.Message;
                }
                return "0";
            }

            ds = this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.Text, queryStr);
            
            try
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception exp)
            {
                errMsg = exp.Message;
            }
            return "0";

        }
        public DataTable ExecuteDataTable(string spName, ref string errMsg, params object[] parameterValues)
        {
            if ((SqlQueryManager.ConStr == null) || (SqlQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] parameterArray1 = this.GetSpParameterSet(SqlQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1).Tables[0];
            }
            return this.ExecuteDataset(SqlQueryManager.ConStr, CommandType.StoredProcedure, spName).Tables[0];

        }

    }
}
