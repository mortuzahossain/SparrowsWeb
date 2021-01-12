using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalEntities.Enums.GlobalEnums;

namespace GlobalEntities.Variables
{
    public static class CommonVariables
    {
        public static DBProvider DBProvider;
        public static string WHName;
        public static string DBConStr;
        public static DBProvider GetDbProvider()
        {
            if (DBProvider == 0)
            {
                DBProvider = (DBProvider)Enum.Parse(typeof(DBProvider), ConfigurationManager.AppSettings["DB_Provider"]);

            }
            return DBProvider;
        }
        public static string GetWHName()
        {
            if (string.IsNullOrEmpty(WHName))
            {
                WHName = "Current Year";
            }
            return WHName;
        }
    }
}
