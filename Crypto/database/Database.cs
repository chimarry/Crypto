using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using Crypto.database.Entity;
using System.Data;

namespace Crypto.database
{       /*Class for working with sqlite database. It offers methods for reading and writing.*/
    class Database
    {
        const string BasePath = "Userbase.sqlite";
        const string createQuery = @"CREATE TABLE IF NOT EXISTS 
                                 [Usertable] ( 
                                               [Username] NVARCHAR(2048) NULL,
                                               [Password] NVARCHAR(2048) NULL,
                                               [CerthPath] NVARCHAR(2048) NULL,
                                               [Folder]    NVARCHAR(2048) NULL )";
        public Database()
        {
        }
        public User Read(string user, string password,string certPath)
        {
            SQLiteConnection conn = new SQLiteConnection("data source =" + BasePath);
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                string getValues = $"SELECT * FROM Usertable WHERE Username==\"{user}\" AND Password=\"{password}\" AND CerthPath=\"{certPath}\"";
                User loggedUser=null;
                using (SQLiteCommand command = new SQLiteCommand(getValues, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        reader.Read();
                         loggedUser = new User(reader["Username"].ToString(), reader["Password"].ToString(), reader["CerthPath"].ToString(), reader["Folder"].ToString());
                    }
                    catch (Exception) { }
                    return loggedUser;
                }
            }
        }
        public static User GetUser(string user)
        {
            SQLiteConnection conn = new SQLiteConnection("data source =" + BasePath);
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                string getValues = $"SELECT * FROM Usertable WHERE Username==\"{user}\"";
                User loggedUser = null;
                using (SQLiteCommand command = new SQLiteCommand(getValues, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        reader.Read();
                        loggedUser = new User(reader["Username"].ToString(), reader["Password"].ToString(), reader["CerthPath"].ToString(), reader["Folder"].ToString());
                    }
                    catch (Exception ex) { }
                    return loggedUser;
                }
            }
        }
        public List<User> ReadAll()
        {
            List<User> list = new List<User>();
            SQLiteConnection conn = new SQLiteConnection("data source =" + BasePath);
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.CommandText = "select * from Usertable";

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User(reader["Username"].ToString(), reader["Password"].ToString(), reader["CerthPath"].ToString(), reader["Folder"].ToString()));
                    }
                }
            }
            return list;
        }

        public bool Write(User user)
        {
            SQLiteConnection conn = new SQLiteConnection("data source =" + BasePath);
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.CommandText = createQuery;
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"SELECT * from Usertable WHERE Username==\"{user.Username}\"";
                if (cmd.ExecuteScalar() == null)
                {
                    cmd.CommandText = $"INSERT INTO Usertable(Username,Password,CerthPath,Folder)values(\"{user.Username}\",\"{user.Password}\",\"{user.CertPath}\",\"{user.Folder}\")";
                    cmd.ExecuteNonQuery();
                    return true;
                }
                else {
                    return false;
                }
            }
        }

    }
}
