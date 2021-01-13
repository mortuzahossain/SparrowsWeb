using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManager.Common;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Collections;
using Oracle.ManagedDataAccess.Client;

namespace DBManager.OracleManager
{
    public class OracleQueryManager
    {
        //    private string connectionString = string.Empty;
        //    private static OracleQueryManager objOracleQueryManager = null;
        //    public OracleQueryManager(string dbConnectionName)
        //    {
        //        DBConnection dbConnection = new DBConnection(dbConnectionName, "", "");
        //        ConnectionManager.GetConnection(ref dbConnection);
        //        connectionString = dbConnection.ConnectionString;
        //    }
        //    public static OracleQueryManager Instance()
        //    {
        //        //if (objOracleQueryManager == null)
        //        //objOracleQueryManager = new LoggerRepository();
        //        return objOracleQueryManager;
        //    }
        //    public bool ReturnData(string Oraclestring, ref DataTable dataTable, ref DBResponse dbResponse)
        //    {
        //        try
        //        {
        //            using (OracleConnection conn = new OracleConnection(connectionString))
        //            {
        //                using (OracleCommand cmd = new OracleCommand(Oraclestring, conn))
        //                {
        //                    cmd.CommandType = CommandType.Text;
        //                    OracleDataAdapter OracleDa = new OracleDataAdapter(cmd);
        //                    OracleDa.Fill(dataTable);
        //                    dbResponse = new DBResponse { Code = 0, Message = "Success", Details = "Ok" };
        //                    return true;
        //                }
        //            }
        //        }
        //        catch (Exception errorExcep)
        //        {
        //            dbResponse = new DBResponse { Code = 1, Message = "Error", Details = errorExcep.Message };
        //        }
        //        return false;
        //    }


        //    public DataSet ReturnDataSet(string Oraclestring)
        //    {
        //        DataSet ds = new DataSet();
        //        try
        //        {
        //            using (OracleConnection conn = new OracleConnection(connectionString))
        //            {
        //                using (OracleCommand cmd = new OracleCommand(Oraclestring, conn))
        //                {
        //                    cmd.CommandTimeout = 10000;
        //                    cmd.CommandType = CommandType.Text;
        //                    OracleDataAdapter OracleDa = new OracleDataAdapter(cmd);
        //                    OracleDa.Fill(ds);
        //                    OracleDa.Dispose();

        //                    cmd.Dispose();
        //                    conn.Dispose();

        //                    return ds;
        //                }
        //            }
        //        }
        //        catch (Exception errorExcep)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            ds.Dispose();
        //        }
        //        return null;
        //    }

        //    internal DataSet ExecuteDataset(string OracleStr, ref string reply, object[] parameterValues)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IDataReader ReturnDataReader(string Oraclestring)
        //    {
        //        IDataReader idr;
        //        OracleConnection conn = new OracleConnection(connectionString);
        //        try
        //        {
        //            OracleConnection.ClearAllPools();
        //            OracleCommand cmd = new OracleCommand(Oraclestring, conn);
        //            if (cmd.Connection.State == ConnectionState.Broken || cmd.Connection.State == ConnectionState.Closed)
        //            {
        //                cmd.Connection.Open();
        //            } 
        //            cmd.CommandType = CommandType.Text;
        //            idr = cmd.ExecuteReader();
        //            //idr.Dispose();
        //        }
        //        catch (Exception errorExcep)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            //idr.Dispose();
        //            //conn.Close();
        //            //conn.Dispose();
        //        }
        //        return idr;
        //    }

        //    internal string ExecuteId(string OracleStr, ref string reply, object[] parameterValues)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        private static string ConStr;
        private OracleConnection OracleCon;
        private OracleDataAdapter adapter;
        private enum OracleConnectionOwnership
        {
            Internal = 0,
            External = 1,
        }
        static OracleQueryManager()
        {
            //OracleQueryManager.paramCache = new Hashtable();
        }
        public OracleQueryManager(string ConnStr)
        {
            this.adapter = new OracleDataAdapter();

            OracleQueryManager.ConStr = ConnStr.ToString();
            this.OracleCon = new OracleConnection(ConStr);
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
        private OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            int num1 = originalParameters.Length - 1;
            OracleParameter[] parameterArray1 = new OracleParameter[num1 + 1];
            int num2 = num1;
            for (int num3 = 0; num3 <= num2; num3++)
            {
                parameterArray1[num3] = (OracleParameter)((ICloneable)originalParameters[num3]).Clone();
            }
            return parameterArray1;

        }
        private OracleParameter[] DiscoverSpParameterSet(OracleConnection connection, string queryStr, ref string errMsg, params object[] parameterValues)
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
            OracleCommand command1 = new OracleCommand(queryStr, connection);
            command1.CommandType = CommandType.Text;
            connection.Open();
            OracleCommandBuilder.DeriveParameters(command1);
            connection.Close();
            //if (!includeReturnValueParameter)
            //{
            //    command1.Parameters.RemoveAt(0);
            //}
            OracleParameter[] parameterArray1 = new OracleParameter[(command1.Parameters.Count - 1) + 1];
            command1.Parameters.CopyTo(parameterArray1, 0);
            foreach (OracleParameter parameter1 in parameterArray1)
            {
                parameter1.Value = DBNull.Value;
            }
            return parameterArray1;

        }
        private void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues, ref string errMsg)
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

        private void ReplaceParameterWithValues(OracleParameter[] commandParameters, object[] parameterValues,ref string SqlStr, ref string errMsg)
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
                        SqlStr = SqlStr.Replace("@"+ commandParameters[num3].ParameterName, "'"+parameterValues[num3].ToString()+"'");
                        
                        //if (parameterValues[num3] is IDbDataParameter)
                        //{
                        //    IDbDataParameter parameter1 = (IDbDataParameter)parameterValues[num3];
                        //    if (parameter1.Value == null)
                        //    {
                        //        commandParameters[num3].Value = DBNull.Value;
                        //    }
                        //    else
                        //    {
                        //        commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameter1.Value);
                        //    }
                        //}
                        //else if (parameterValues[num3] == null)
                        //{
                        //    commandParameters[num3].Value = DBNull.Value;
                        //}
                        //else
                        //{
                        //    commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameterValues[num3]);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = string.Empty;
                errMsg = "Error:" + ex.Message;
            }

        }


        private void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, ref bool mustCloseConnection)
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
        private void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandParameters != null)
            {
                foreach (OracleParameter parameter1 in commandParameters)
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

        private OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName, ref string errMsg)
        {
            OracleParameter[] parameterArray1;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            try
            {
                this.OracleCon = (OracleConnection)((ICloneable)connection).Clone();
                errMsg = string.Empty;
                parameterArray1 = this.GetParameter(this.OracleCon, spName, ref errMsg).ToArray();
            }
            finally
            {
                if (this.OracleCon != null)
                {
                    this.OracleCon.Dispose();
                }
            }
            return parameterArray1;

        }
        private List<OracleParameter> GetParameter(OracleConnection connection, string queryStr, ref string errMsg)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("queryStr");
            }
            string text1 = queryStr; //+ this.IIf(includeReturnValueParameter, ":include ReturnValue Parameter", "").ToString();
            string[] strPar = queryStr.Split('@');
            List<OracleParameter> OracleParamList = new List<OracleParameter>();
            for (int i = 1; i < strPar.Length; i++)
            {
                string[] param = strPar[i].Split(' ');
                OracleParameter Oracleparam = new OracleParameter(param[0].ToString(), "");
                OracleParamList.Add(Oracleparam);
            }
            return OracleParamList;
            //OracleParameter[] parameterArray1 = (OracleParameter[])OracleQueryManager.paramCache[text1];

            //if (parameterArray1 == null)
            //{
            //    OracleParameter[] parameterArray2 = this.DiscoverSpParameterSet(connection, queryStr, ref errMsg, new object[0]);
            //    OracleQueryManager.paramCache[text1] = parameterArray2;
            //    parameterArray1 = parameterArray2;
            //}
            //return this.CloneParameters(parameterArray1);

        }
        private OracleParameter[] GetSpParameterSet(string connectionstring, string queryStr, ref string errMsg)
        {
            OracleParameter[] parameterArray1;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.OracleCon = new OracleConnection(connectionstring);
                errMsg = string.Empty;
                parameterArray1 = this.GetParameter(this.OracleCon, queryStr, ref errMsg).ToArray();
            }
            finally
            {
                if (this.OracleCon != null)
                {
                    this.OracleCon.Dispose();
                }
            }
            return parameterArray1;

        }

        private int ExecuteNonQuery(OracleConnection connection, string spName, ref string errMsg, params object[] parameterValues)
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
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(connection, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return 0;// this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, parameterArray1);
            }
            return 0;// this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, new OracleParameter[0]);
        }

        //
        public OracleParameter[] ExecuteNonQueryParam(string spName, ref string reply, params object[] parameterValues)
        {
            OracleParameter[] param = null;
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, spName, ref reply);
                if (parameterArray1 != null)
                {
                    this.AssignParameterValues(parameterArray1, parameterValues, ref reply);
                    param = this.ExecuteNonQueryParam(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
                }
            }
            return param;
        }
        //
        public OracleParameter[] ExecuteNonQueryParam(OracleConnection connection, string spName, ref string reply, params object[] parameterValues)
        {
            OracleParameter[] param = null;
            OracleConnection OracleCon = null;

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
                    OracleCon = (OracleConnection)((ICloneable)connection).Clone();

                    OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleCon, spName, ref reply);
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
        private OracleParameter[] ExecuteNonQueryParam(string constr, CommandType commandType, string commandText, ref string reply, params OracleParameter[] commandParameters)
        {
            OracleParameter[] param = null;
            OracleConnection OracleCon = null;

            try
            {
                OracleCon = new OracleConnection(constr);
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
        private OracleParameter[] ExecuteNonQueryParam(OracleConnection connection, CommandType commandType, string commandText, ref string reply, params OracleParameter[] commandParameters)
        {
            OracleParameter[] param = null;
            try
            {
                if (connection == null)
                {
                    throw new ArgumentNullException("connection");
                }
                OracleCommand command1 = new OracleCommand();
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

        private int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            OracleCommand command1 = new OracleCommand();
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
        private int ExecuteNonQuery(string connectionstring, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            int num1;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.OracleCon = new OracleConnection(connectionstring);
                this.OracleCon.Open();
                num1 = this.ExecuteNonQuery(this.OracleCon, commandType, commandText, commandParameters);
            }
            finally
            {
                if (this.OracleCon != null)
                {
                    this.OracleCon.Dispose();
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
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteNonQuery(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1);
            }
            else
            {
                return this.ExecuteNonQuery(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName);
            }

        }
        private OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, OracleQueryManager.OracleConnectionOwnership connectionOwnership)
        {
            OracleDataReader dataReader;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool flag1 = false;
            OracleCommand command1 = new OracleCommand();
            try
            {
                OracleDataReader reader2;
                this.PrepareCommand(command1, connection, transaction, commandType, commandText, commandParameters, ref flag1);
                if (connectionOwnership == OracleQueryManager.OracleConnectionOwnership.External)
                {
                    reader2 = command1.ExecuteReader();
                }
                else
                {
                    reader2 = command1.ExecuteReader(CommandBehavior.CloseConnection);
                }
                bool flag2 = true;
                foreach (OracleParameter parameter1 in command1.Parameters)
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
        private OracleDataReader ExecuteReader(string connectionstring, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(connectionstring, commandType, commandText, null);

        }
        private OracleDataReader ExecuteReader(string connectionstring, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleDataReader dataReader;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.OracleCon = new OracleConnection(connectionstring);
                this.OracleCon.Open();
                dataReader = this.ExecuteReader(this.OracleCon, null, commandType, commandText, commandParameters, OracleQueryManager.OracleConnectionOwnership.Internal);
            }
            catch (Exception)
            {
                if (this.OracleCon != null)
                {
                    this.OracleCon.Dispose();
                }
                return null;
            }
            return dataReader;

        }
        public OracleDataReader ExecuteReader(string spName, ref string errMsg, params object[] parameterValues)
        {
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteReader(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1);
            }
            return this.ExecuteReader(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName);

        }
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionstring, commandType, commandText, null);
        }
        private DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            OracleCommand command1 = new OracleCommand();
            DataSet dataSet = new DataSet();
            bool flag1 = false;
            this.PrepareCommand(command1, connection, null, commandType, commandText, commandParameters, ref flag1);
            try
            {
                this.adapter = new OracleDataAdapter(command1);
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
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            DataSet dataSet;
            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                this.OracleCon = new OracleConnection(connectionstring);
                this.OracleCon.Open();
                dataSet = this.ExecuteDataset(this.OracleCon, commandType, commandText, null);
            }
            finally
            {
                if (this.OracleCon != null)
                {
                    this.OracleCon.Dispose();
                }
            }
            return dataSet;

        }
        public DataSet ExecuteDataset(string queryStr, ref string errMsg, params object[] parameterValues)
        {
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }

            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, queryStr, ref errMsg);
                //this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                this.ReplaceParameterWithValues(parameterArray1, parameterValues,ref queryStr,  ref errMsg);
                return this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.Text, queryStr, parameterArray1);
            }
            return this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.Text, queryStr);

        }
        public string ExecuteSingleValue(string queryStr, ref string errMsg, params object[] parameterValues)
        {
            DataSet ds;
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }

            if ((queryStr == null) || (queryStr.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, queryStr, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                this.ReplaceParameterWithValues(parameterArray1, parameterValues, ref queryStr, ref errMsg);
                ds = this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.Text, queryStr, parameterArray1);
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

            ds = this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.Text, queryStr);

            try
            {
                return ds.Tables[0].Rows[0]["Id"].ToString();
            }
            catch (Exception exp)
            {
                errMsg = exp.Message;
            }
            return "0";

        }
        public DataTable ExecuteDataTable(string spName, ref string errMsg, params object[] parameterValues)
        {
            if ((OracleQueryManager.ConStr == null) || (OracleQueryManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleQueryManager.ConStr, spName, ref errMsg);
                this.AssignParameterValues(parameterArray1, parameterValues, ref errMsg);
                return this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1).Tables[0];
            }
            return this.ExecuteDataset(OracleQueryManager.ConStr, CommandType.StoredProcedure, spName).Tables[0];

        }

    }
}
