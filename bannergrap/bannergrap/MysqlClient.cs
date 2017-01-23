using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace bannergrap
{
    class MysqlClient
    {
        MySqlConnection connection = null;
        public bool Connect(UInt32 ip, UInt16 port, int timeout)
        {
            string server = "localhost";
            string database = "connectcsharptomysql";
            string uid = "username";
            string password = "password";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }
}
