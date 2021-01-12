using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Common
{
    public class DBConnection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string DataSource { get; set; }
        public string DBPort { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; } 
        public string ConnectionString { get; set; }
        public string DBWHName { get; set; }
        public string RespMsg { get; internal set; }

        public DBConnection(string dbName, string dbWHName,string dbProvider)
        {
            Name = dbName;
            DBWHName = dbWHName;
            Provider = dbProvider;
        }
    }
   

}
