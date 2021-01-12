using System.Collections.Generic;

namespace DBManager.Common
{
    public static class ConnectionManager
    {
        static string conn = string.Empty;
        static ConnectionProvider conProvider = new ConnectionProvider();
        public static List<DBConnection> DBConnectionList = new List<DBConnection>();
        public static bool GetConnection(ref DBConnection dbConnection)
        {
            if (DBConnectionList != null)
            {
                foreach (DBConnection dbCon in DBConnectionList)
                {
                    if (dbCon != null)
                    {
                        if ((dbCon.Name == dbConnection.Name) && (dbCon.Provider == dbConnection.Provider))// && (dbConnection.DBWHName == dbCon.DBWHName))
                        {
                            dbConnection = dbCon;
                            return true;
                        }
                    }
                }
            }

            conProvider.GetDBConnection(ref dbConnection);
            if (!string.IsNullOrEmpty(dbConnection.ConnectionString))
            {
                DBConnectionList.Add(dbConnection);
                return true;
            }

            return false;
        }
    }
}
