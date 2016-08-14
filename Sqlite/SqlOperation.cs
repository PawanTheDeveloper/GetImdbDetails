using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Finisar.SQLite;

/*
 * A Third Party dll is used to connect to SQL Lite.
 * http://adodotnetsqlite.sourceforge.net/documentation/
 * This provides all the dll for the project including all the Documentation.
 * Refer http://adodotnetsqlite.sourceforge.net/documentation/csharp_example.php for code Example.
 * Sqlite creates a Database file (.db), this file can be opened in a Firefox Addon
 * Install Firefox and install Sqlite Manager Add-on from https://addons.mozilla.org/En-us/firefox/addon/sqlite-manager/
 * To open a database file , Goto Firefox(upper left Corner)->WebDeveloper->SQLiteManager.
 * */
namespace SqlLite
{
    public class SqlOperation
    {
        private SQLiteConnection sqlite_conn;
        private SQLiteCommand sqlite_cmd;
        private string SqlDatabaseFile = string.Empty;
        private void Connect()
        {
            try
            {
                sqlite_conn = new SQLiteConnection("Data Source=" + SqlDatabaseFile + ".db" + ";Version=3;New=True;Compress=True;");
                sqlite_conn.Open();
                sqlite_cmd = sqlite_conn.CreateCommand(); 
            }
            catch (SQLiteException excep)
            {

            }
        }
        private void close()
        {
            sqlite_conn.Close();
        }
        #region Public Memebers
   
        public void CloseCOnnection()
        {
            close();
        }
        public SQLiteDataReader SqlOutputQuery(string query)
        {
            SQLiteDataReader reader = null;
            try
            {
                sqlite_cmd.CommandText = "SELECT * FROM test";
                reader = sqlite_cmd.ExecuteReader();
                return reader;
            }
            catch (SQLiteException excep)
            {
                return reader;
            }
            
        }
        public bool SqlInputQuery(string query)
        {
            try
            {
                sqlite_cmd.CommandText = query;
                sqlite_cmd.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException except)
            {
                return false;
            }
        }
        public SqlOperation(string DatabasePath)
        {
            this.SqlDatabaseFile = DatabasePath;
        }
        #endregion
    }
}
