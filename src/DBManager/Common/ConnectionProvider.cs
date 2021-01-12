using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using FloraBank.Core;
using static GlobalEntities.Enums.GlobalEnums;

namespace DBManager.Common
{
    public class ConnectionProvider
    {
        public void GetDBConnection(ref DBConnection dbConnection)
        {
            GetConnection(ref dbConnection);
        }

        #region ***** Single DES *****
        static byte[] Key = ASCIIEncoding.ASCII.GetBytes("fslsmblk");
        static byte[] IV = ASCIIEncoding.ASCII.GetBytes("sfmsbllv");

        private string DES_Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
        #endregion

        private bool ParseXMLFile(ref DBConnection dbCon)
        {
            try
            {
                //XmlDocument doc = new XmlDocument();
                //DBConnection dbCon = new DBConnection("", "","");
                //doc.Load(System.Windows.Forms.Application.StartupPath + "\\App Configuration\\BACH_CONNECTION.xml");
                //doc.Load(ConfigurationManager.ConnectionStrings["ConFilePath"].ConnectionString);

                //doc.Load(@"D:\Flora Bank\Release\Flora Bank\CONNECTION.xml");
                //doc.Load(ConfigurationManager.AppSettings["ConFilePath"]);
                //XmlNodeList bookList = doc.GetElementsByTagName("CONNECTION");


                //int i = 0;
                //foreach (XmlNode node in bookList)
                //{
                //    XmlElement bookElement = (XmlElement)node;

                //    string str = DES_Decrypt(bookElement.GetElementsByTagName("NAME")[0].InnerText);
                //    string str2 = DES_Decrypt(bookElement.GetElementsByTagName("CONNECTIONTYPE")[0].InnerText);

                //    if ((dbCon.Name == DES_Decrypt(bookElement.GetElementsByTagName("NAME")[0].InnerText)) && (dbCon.Provider == DES_Decrypt(bookElement.GetElementsByTagName("CONNECTIONTYPE")[0].InnerText)))
                //    {
                //        dbCon.Id = i;
                //        dbCon.Name = DES_Decrypt(bookElement.GetElementsByTagName("NAME")[0].InnerText);
                //        dbCon.Provider = DES_Decrypt(bookElement.GetElementsByTagName("CONNECTIONTYPE")[0].InnerText);
                //        dbCon.DataSource = DES_Decrypt(bookElement.GetElementsByTagName("DATASOURCE")[0].InnerText);
                //        //dbCon.DBPort = DES_Decrypt(bookElement.GetElementsByTagName("PORT")[0].InnerText);
                //        dbCon.Database = DES_Decrypt(bookElement.GetElementsByTagName("DATABASE")[0].InnerText);
                //        dbCon.UserName = DES_Decrypt(bookElement.GetElementsByTagName("USERNAME")[0].InnerText).Replace("Secret", "");
                //        dbCon.Password = DES_Decrypt(bookElement.GetElementsByTagName("PASSWORD")[0].InnerText).Replace("Secret", "");
                //        dbCon.Status = DES_Decrypt(bookElement.GetElementsByTagName("STATUS")[0].InnerText);
                //        dbCon.DBWHName = DES_Decrypt(bookElement.GetElementsByTagName("NAME")[0].InnerText);

                //        if (dbCon.Provider == DBProvider.SqlClient.ToString())
                //        {
                //            dbCon.ConnectionString = "Data Source = " + dbCon.DataSource + "; Initial Catalog=" + dbCon.Database + ";Persist Security Info=True;User ID=" + dbCon.UserName + ";Password=" + dbCon.Password;
                //        }
                //        else if (dbCon.Provider == DBProvider.Oracle.ToString())
                //        {
                //            //dbCon.ConnectionString = "Data Source = " + dbCon.DataSource + "/" + dbCon.Database + ";Persist Security Info=True;user id=" + dbCon.UserName + ";password=" + dbCon.Password + "; Unicode=True";
                //            dbCon.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dbCon.DataSource + ")(PORT=" + dbCon.DBPort + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + dbCon.Database + ")));User Id=" + dbCon.UserName + ";Password=" + dbCon.Password + ";Persist Security Info=True;";
                //        }

                //        return true;
                //    }
                //}
                // Development
                dbCon.ConnectionString = "Data Source = 172.16.22.24; Initial Catalog = sparrows; User ID = sa; Password = 123";
                //Live
                //dbCon.ConnectionString = "Data Source = 172.16.22.24; Initial Catalog = SparrowsLive; User ID = sa; Password = 123";
                return true;

            }
            catch (Exception errorException)
            {
                throw errorException;
            }
            return false;
        }

        public void GetConnection(ref DBConnection dbConnection)
        {

            if (dbConnection.ConnectionString == null)
            {
                ParseXMLFile(ref dbConnection);


            }
            UpdateUser(ref dbConnection);  

            //string sqlstring = "select database_name,server_name,db_user,db_pwd from wv_parameter_report_server where database_name='"+ bdName + "' and period_code='" + connectionStringYear + "'";
            //try
            //{
            //    DataTable dt = new DataTable();
            //    using (SqlConnection conn = new SqlConnection(dbConnection.ConnectionString))
            //    {
            //        using (SqlCommand cmd = new SqlCommand(sqlstring, conn))
            //        {
            //            cmd.CommandType = CommandType.Text;
            //            SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
            //            sqlDa.Fill(dt);

            //            DBConnection dbCon = new DBConnection(bdName, connectionStringYear, dbProvider);

            //            if (dt != null)
            //            {
            //                dbCon.Name = bdName;
            //                dbCon.Provider = "SqlClient";// dt.Rows[0]["database_name"].ToString();
            //                dbCon.DataSource = dt.Rows[0]["server_name"].ToString();
            //                dbCon.Database = dt.Rows[0]["database_name"].ToString();
            //                dbCon.UserName = dt.Rows[0]["db_user"].ToString();
            //                dbCon.Password = dt.Rows[0]["db_pwd"].ToString();
            //                dbCon.Status = "1";
            //                dbCon.DBWHName = connectionStringYear;

            //                if (SystemSecurity.IsEncrypted(dbCon.Password))
            //                {
            //                    dbCon.Password = SystemSecurity.DecryptValue(dbCon.Password);
            //                }
            //                if (SystemSecurity.IsEncrypted(dbCon.UserName))
            //                {
            //                    dbCon.UserName = SystemSecurity.DecryptValue(dbCon.UserName);
            //                }

            //                if (dbCon.Provider == DBProvider.SqlClient.ToString())
            //                {
            //                    dbCon.ConnectionString = "Data Source=" + dbCon.DataSource + "; Initial Catalog=" + dbCon.Database + ";Persist Security Info=True;User ID=" + dbCon.UserName + ";Password=" + dbCon.Password;
            //                }
            //                else if (dbCon.Provider == DBProvider.Oracle.ToString())
            //                {
            //                    dbCon.ConnectionString = "Data Source=" + dbCon.DataSource + ";Persist Security Info=True;user id=" + dbCon.UserName + ";password=" + dbCon.Password + ";";
            //                }

            //                return dbCon;

            //            }
            //        }
            //    }
            //}
            //catch (Exception errorExcep)
            //{

            //}
            //return null;
        }

        public void UpdateUser(ref DBConnection dbConnection)
        {
            if (dbConnection.ConnectionString == null)
            {
                ParseXMLFile(ref dbConnection);
            }
            string sqlstring = "select netappuser, netappass, netwebuser, netwebpass, centralip from Florabank_online.dbo.wv_net_system36";
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(dbConnection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlstring, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
                        sqlDa.Fill(dt);


                        if (dt != null)
                        {

                            if (dt.Rows.Count > 0)
                            {


                                dbConnection.UserName = dt.Rows[0]["netappuser"].ToString();
                                dbConnection.Password = dt.Rows[0]["netappass"].ToString();


                                if (SystemSecurity.IsEncrypted(dbConnection.Password))
                                {
                                    dbConnection.Password = SystemSecurity.DecryptValue(dbConnection.Password);
                                }
                                if (SystemSecurity.IsEncrypted(dbConnection.UserName))
                                {
                                    dbConnection.UserName = SystemSecurity.DecryptValue(dbConnection.UserName);
                                }
                                if (dbConnection.Provider == DBProvider.SqlClient.ToString())
                                {
                                    dbConnection.ConnectionString = "Data Source=" + dbConnection.DataSource + "; Initial Catalog=" + dbConnection.Database + ";Persist Security Info=True;User ID=" + dbConnection.UserName + ";Password=" + dbConnection.Password;
                                }
                                else if (dbConnection.Provider == DBProvider.Oracle.ToString())
                                {
                                    dbConnection.ConnectionString = "Data Source=" + dbConnection.DataSource + ";Persist Security Info=True;user id=" + dbConnection.UserName + ";password=" + dbConnection.Password + ";";
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception errorExcep)
            {
                dbConnection.RespMsg = errorExcep.Message;
            }

        }
    }

}
