using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using Oracle.DataAccess.Client;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static GlobalEntities.Enums.GlobalEnums;
using DBManager.Common; 
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using GlobalEntities.Entities;

namespace DBManager.OracleManager
{
    public class OracleProcedureManager//OracleProcedureManager
    {
        private enum OracleConnectionOwnership
        {
            Internal = 0,
            External = 1,
        }
        //private static string ConStr;
        //private OracleConnection OracleCon;
        //private OracleDataAdapter adapter;
        //private Hashtable paramCache;

        private static string ConStr;
        private OracleConnection SqlCon;
        private OracleDataAdapter adapter;
        private static Hashtable paramCache;


        static OracleProcedureManager()
        {
            //OracleProcedureProvider.paramCache = new Hashtable();
        }
        public OracleProcedureManager(string ConnStr)
        {
            ////adapter = new OracleDataAdapter();
            ////OracleProcedureManager.ConStr = ConnStr.ToString();
            ////OracleCon = new OracleConnection(ConStr);

            ////if ((dbCon.Name == dbConnection.Name) && (dbCon.DatabaseYear == dbConnection.DatabaseYear))
            //DBConnection dBConnection = new DBConnection(ConnStr.ToString(),"");
            //ConnectionManager.GetConnection();

            this.adapter = new OracleDataAdapter();

            OracleProcedureManager.ConStr = ConnStr.ToString();
            this.SqlCon = new OracleConnection(ConStr);

        }

        public OracleProcedureManager()
        {
            //adapter = new OracleDataAdapter();
            //OracleCon = new OracleConnection();

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
        private OracleParameter[] DiscoverSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter, ref string reply, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            OracleParameter[] parameterArray1 = null;

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Open)
                {
                    OracleCommand command1 = new OracleCommand(spName, connection);
                    command1.CommandType = CommandType.StoredProcedure;

                    OracleCommandBuilder.DeriveParameters(command1);

                    if (!includeReturnValueParameter)
                    {
                        command1.Parameters.RemoveAt(0);
                    }
                    parameterArray1 = new OracleParameter[(command1.Parameters.Count - 1) + 1];
                    command1.Parameters.CopyTo(parameterArray1, 0);
                    foreach (OracleParameter parameter1 in parameterArray1)
                    {
                        parameter1.Value = DBNull.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                reply = "Error:" + ex.Message;
                return parameterArray1;
            }
            return parameterArray1;

        }
        private void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            int num1 = commandParameters.Length - 1;
            int num2 = parameterValues.Length;
            try
            {
                if ((commandParameters != null) || (parameterValues != null))
                {
                    int j = 0;
                    for (int num3 = 0; num3 <= num1; num3++)
                    {

                        if (commandParameters[num3].Direction != ParameterDirection.Output)
                        {
                            for (int n = j; n < j + 1; n++)
                            {
                                commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameterValues[n]);

                            }
                            j++;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }
        private void AssignParameterValuesForReturnRS(List<OracleParameter> commandParameters, object[] parameterValues)
        {
            int num1 = commandParameters.Count - 1;
            int num2 = parameterValues.Length;
            try
            {
                if ((commandParameters != null) || (parameterValues != null))
                {
                    if (commandParameters.Count > 0)
                    {
                        int j = 0;
                        for (int num3 = 0; num3 <= num1; num3++)
                        {

                            if (commandParameters[num3].Direction != ParameterDirection.Output)
                            {
                                for (int n = j; n < j + 1; n++)
                                {
                                    commandParameters[num3].Value = RuntimeHelpers.GetObjectValue(parameterValues[n]);

                                }
                                j++;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < parameterValues.Length; i++)
                        {
                            if (RuntimeHelpers.GetObjectValue(parameterValues[i])==null)
                            {
                                break;
                            }
                            OracleParameter param = new OracleParameter
                            {
                                Value = RuntimeHelpers.GetObjectValue(parameterValues[i]),
                                ParameterName = "vParam" + (i + 1).ToString(),
                                Direction = ParameterDirection.Input,
                                OracleDbType = OracleDbType.Varchar2
                            };
                            commandParameters.Add(param);
                        }
                        OracleParameter cursor = new OracleParameter
                        {
                            ParameterName = "REFCURSOR",
                            Direction = ParameterDirection.Output,
                            OracleDbType = OracleDbType.RefCursor
                        };

                        commandParameters.Add(cursor);
                    }
                }
            }
            catch (Exception)
            {

            }

        }
        private void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType,
            string commandText, OracleParameter[] commandParameters, ref bool mustCloseConnection)
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
                mustCloseConnection = false;
            }
            else
            {
                mustCloseConnection = true;
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
            else
            {
                OracleParameter cursor = command.Parameters.Add(
                new OracleParameter
                {
                    ParameterName = "REFCURSOR",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                }
            );
            }

        }
        //
        private OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName, ref string reply)
        {
            return GetSpParameterSet(connection, spName, true, ref reply);
        }
        private OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter, ref string reply)
        {
            OracleParameter[] parameterArray1;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            try
            {
                OracleConnection OracleCon = (OracleConnection)((ICloneable)connection).Clone();

                parameterArray1 = this.GetSpParameterSetInternal(OracleCon, spName, includeReturnValueParameter, ref reply);
            }
            finally
            {
                //if (OracleCon != null)
                //{
                //    OracleCon.Dispose();
                //}
            }
            return parameterArray1;

        }
        private OracleParameter[] GetSpParameterSetInternal(OracleConnection connection, string spName, bool includeReturnValueParameter, ref string reply)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            else if (connection.State == System.Data.ConnectionState.Open)
            {
                //OracleConnection OracleCon = (OracleConnection)((ICloneable)connection).Clone();
                paramCache = new Hashtable();

                string text1 = connection.ConnectionString + ":" + spName + this.IIf(includeReturnValueParameter, ":include ReturnValue Parameter", "").ToString();
                OracleParameter[] parameterArray1 = (OracleParameter[])paramCache[text1];
                if (parameterArray1 == null)
                {
                    OracleParameter[] parameterArray2 = this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter, ref reply, new object[0]);
                    paramCache[text1] = parameterArray2;
                    parameterArray1 = parameterArray2;
                }
                if (parameterArray1 != null)
                    return this.CloneParameters(parameterArray1);
                else
                    return null;
            }

            return null;
        }
        private OracleParameter[] GetSpParameterSet(string connectionstring, string spName, bool includeReturnValueParameter, ref string reply)
        {
            OracleParameter[] parameterArray1;
            OracleConnection OracleCon = null;

            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {

                OracleCon = new OracleConnection(connectionstring);
                OracleCon.Open();
                parameterArray1 = this.GetSpParameterSetInternal(OracleCon, spName, includeReturnValueParameter, ref reply);

            }
            catch 
            {
                parameterArray1 = null;
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
            return parameterArray1;

        }
        private OracleParameter[] GetSpParameterSet(string connectionstring, string spName, ref string reply)
        {
            return GetSpParameterSet(connectionstring, spName, true, ref reply);
        }
        //
        private int ExecuteNonQuery(OracleConnection connection, string spName, ref string reply, params object[] parameterValues)
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
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Open)
                {
                    OracleConnection OracleCon = (OracleConnection)((ICloneable)connection).Clone();

                    OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleCon, spName, ref reply);

                    if (parameterArray1 != null)
                    {
                        this.AssignParameterValues(parameterArray1, parameterValues);
                        return this.ExecuteNonQuery(OracleCon, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
                    }
                }
            }
            return 0;
        }
        private int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, ref string reply, params OracleParameter[] commandParameters)
        {
            int num1 = 0;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            OracleCommand command1 = new OracleCommand();
            bool flag1 = false;

            try
            {
                this.PrepareCommand(command1, connection, null, commandType, commandText, commandParameters, ref flag1);
                num1 = command1.ExecuteNonQuery();
                command1.Parameters.Clear();
                //if (flag1)
                //{
                //    connection.Close();
                //}
                reply = "Success";
            }
            catch (Exception ex)
            {
                reply = "Error:" + ex.Message;
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
            return num1;
        }

        private int ExecuteNonQuery(string connectionstring, CommandType commandType, string commandText, ref string reply, params OracleParameter[] commandParameters)
        {
            int num1;
            OracleConnection OracleCon = null;

            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                OracleCon = new OracleConnection(connectionstring);
                OracleCon.Open();
                num1 = this.ExecuteNonQuery(OracleCon, commandType, commandText, ref reply, commandParameters);
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
            return num1;

        }
        private int ExecuteNonQuery(string connectionstring, CommandType commandType, string commandText, ref string reply)
        {
            return ExecuteNonQuery(connectionstring, commandType, commandText, ref reply, null);
        }
        public int ExecuteNonQuery(string spName, ref string reply, params object[] parameterValues)
        {
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                reply = "Error: Connection string.";
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                reply = "Error: Procedure name not found.";
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
                this.AssignParameterValues(parameterArray1, parameterValues);
               return this.ExecuteNonQuery(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
            }
            else
            {
               return this.ExecuteNonQuery(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply);
            }

        }

    
        //
        public OracleParameter[] ExecuteNonQueryParam(string spName, ref string reply, params object[] parameterValues)
        {
            OracleParameter[] param = null;
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
                if (parameterArray1 != null)
                {
                    this.AssignParameterValues(parameterArray1, parameterValues);
                    param = this.ExecuteNonQueryParam(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
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
                        this.AssignParameterValues(parameterArray1, parameterValues);
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
            OracleCommand oraCommand = null;

            try
            {
                if (connection == null)
                {
                    throw new ArgumentNullException("connection");
                }
                oraCommand = new OracleCommand();
                bool flag1 = false;
                this.PrepareCommand(oraCommand, connection, null, commandType, commandText, commandParameters, ref flag1);
                int num1 = oraCommand.ExecuteNonQuery();
                oraCommand.Parameters.Clear();
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
                //
                if (oraCommand != null)
                {
                    oraCommand.Dispose();
                }
                //
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return commandParameters;
            }
            finally
            {
                if (oraCommand != null)
                {
                    oraCommand.Dispose();
                }
                //
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
        private OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText,
        OracleParameter[] commandParameters, OracleProcedureManager.OracleConnectionOwnership connectionOwnership)
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
                if (connectionOwnership == OracleProcedureManager.OracleConnectionOwnership.External)
                {
                    reader2 = command1.ExecuteReader();
                }
                else
                {
                    reader2 = command1.ExecuteReader();//CommandBehavior.CloseConnection);
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
            catch (Exception err)
            {
                //if (flag1)
                //{
                //    connection.Close();
                //}
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
            OracleConnection OracleCon = null;

            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                OracleCon = new OracleConnection(connectionstring);
                OracleCon.Open();
                dataReader = this.ExecuteReader(OracleCon, null, commandType, commandText, commandParameters, OracleProcedureManager.OracleConnectionOwnership.Internal);
            }
            catch (Exception)
            {
                if (OracleCon != null)
                {
                    if (OracleCon.State == ConnectionState.Open)
                    {
                        OracleCon.Close();
                    }
                }
                return null;
            }
            return dataReader;

        }
        public OracleDataReader ExecuteReader(string spName, ref string reply, params object[] parameterValues)
        {
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
                this.AssignParameterValues(parameterArray1, parameterValues);
                //this.AssignParameterValuesForReturnRS(parameterArray1, parameterValues);
                return this.ExecuteReader(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, parameterArray1.ToArray());
            }
            return this.ExecuteReader(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName);

        }
        //**//
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText, ref string reply)
        {
            OracleParameter[] commandParameters = this.GetSpParameterSet(OracleProcedureManager.ConStr, commandText, ref reply);
            return ExecuteDataset(connectionstring, commandType, commandText, ref reply, commandParameters);
        }
        private DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, ref string replyMsg,
        params OracleParameter[] commandParameters)
        {
            OracleDataAdapter adapter = null;

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            OracleCommand oraCommand = new OracleCommand();
            oraCommand.CommandTimeout = 30 * 60;
            DataSet dataSet = new DataSet();
            bool flag1 = false;

            this.PrepareCommand(oraCommand, connection, null, commandType, commandText, commandParameters, ref flag1);

            try
            {
                

                
                adapter = new OracleDataAdapter(oraCommand);
                adapter.Fill(dataSet);
                oraCommand.Parameters.Clear();
                replyMsg = "Success";
            }
            catch (Exception ex)
            {
                replyMsg = "Error:" + ex.Message;
                //
                if (oraCommand != null)
                {
                    oraCommand.Dispose();
                }
                //
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                //
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            finally
            {
                //
                if (oraCommand != null)
                {
                    oraCommand.Dispose();
                }
                //
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                //
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dataSet;

        }
        private DataSet ExecuteDataset(string connectionstring, CommandType commandType, string commandText, ref string replyMsg,
        params OracleParameter[] commandParameters)
        {
            DataSet dataSet;
            OracleConnection OracleCon = null;

            if ((connectionstring == null) || (connectionstring.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            try
            {
                OracleCon = new OracleConnection(connectionstring);
                OracleCon.Open();
                dataSet = this.ExecuteDataset(OracleCon, commandType, commandText, ref replyMsg, commandParameters);
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
            return dataSet;

        }
        public DataSet ExecuteDataset(string spName, ref string reply, params object[] parameterValues)
        {
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
                if (!reply.Contains("Error"))
                {
                    this.AssignParameterValues(parameterArray1, parameterValues);
                    return this.ExecuteDataset(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1);
                }
                else
                    return null;
            }
            return this.ExecuteDataset(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply);

        }
        public DataTable ExecuteDataTable(string spName, ref string reply, params object[] parameterValues)
        {
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
                if (!reply.Contains("Error"))
                {
                    //this.AssignParameterValuesForReturnRS(parameterArray1, parameterValues);
                    this.AssignParameterValues(parameterArray1, parameterValues);
                    return this.ExecuteDataset(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply, parameterArray1.ToArray()).Tables[0];
                }
                else
                    return null;
            }
            return this.ExecuteDataset(OracleProcedureManager.ConStr, CommandType.StoredProcedure, spName, ref reply).Tables[0];

        }

        //public DataTable ExecuteDataTableTest(string spName, ref string reply, params object[] parameterValues)
        //{
        //    try
        //    {
        //        using (OracleConnection conn = new OracleConnection(OracleProcedureManager.ConStr))
        //        {
        //            using (OracleCommand cmd = new OracleCommand("aa_test_sp", conn))
        //            {
        //                if (conn.State.ToString() != "Open")
        //                    conn.Open();
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                OracleParameter Param = new OracleParameter("o_rc", OracleType.Cursor, 1000, "o_rc");
        //                Param.Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add(Param);
        //                OracleDataAdapter myAddapter = new OracleDataAdapter(cmd);
        //                DataTable dTable = new DataTable();
        //                myAddapter.Fill(dTable);
        //                if (dTable != null)
        //                {
        //                    if (dTable.Rows.Count > 0)
        //                    {
        //                        return dTable;

        //                    }
        //                }
        //            }
        //        }

        //        //OracleConnection conn = new OracleConnection();
        //        //conn.ConnectionString = OracleProcedureManager.ConStr;
        //        //conn.Open();
        //        //OracleCommand command = conn.CreateCommand();
        //        //command.CommandText = "select accountno, acname from CUS_AC_1";
        //        //command.CommandType = CommandType.Text;
        //        //OracleDataAdapter a = new OracleDataAdapter(command);
        //        //DataTable t = new DataTable();
        //        //a.Fill(t);
        //        //command.Dispose();

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        public List<SPParameter> GetSPParameters(string spName, ref string reply)
        {
            List<SPParameter> paramList = new List<SPParameter>();
            if ((OracleProcedureManager.ConStr == null) || (OracleProcedureManager.ConStr.Length == 0))
            {
                throw new ArgumentNullException("connectionstring");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }

        


            OracleParameter[] parameterArray1 = this.GetSpParameterSet(OracleProcedureManager.ConStr, spName, ref reply);
            for (int i = 0; i < parameterArray1.Length; i++)
            {
                SPParameter reportSQL = new SPParameter();
                reportSQL.Id = i + 1;
                reportSQL.Name = parameterArray1[i].ParameterName;
                reportSQL.Length = parameterArray1[i].Size;
                reportSQL.Type = parameterArray1[i].DbType.ToString();
                reportSQL.Value = parameterArray1[i].Value.ToString();
                paramList.Add(reportSQL);

            }

            return paramList;

        }
        public OracleConnection OpenConnection(string constr)
        {
            OracleConnection OracleCon = new OracleConnection(constr);
            OracleCon.Open();
            return OracleCon;
        }
        public void CloseConnection(OracleConnection OraCon)
        {
            if (OraCon != null)
            {
                OraCon.Close();
                OraCon.Dispose();
            }
        }

    }
}
