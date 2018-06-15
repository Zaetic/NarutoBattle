﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Controllers.DAL
{
    public class Context
    {
        SQLiteConnection conn;
        public Context()
        {
            conn = new SQLiteConnection("Data Source=|DataDirectory|NBDB.db;Version=3;");
        }

        public List<string> return_Characters()
        {
            List<string> chars = new List<string>();
            conn.Open();
    
            string sql = "SELECT Name FROM Character";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        chars.Add(rdr["Name"].ToString());
                    }
                }
            }
            conn.Close();
            return chars;
        }

        public object teste()
        {
            conn.Open();
            string sql = "SELECT Name FROM Character";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            //dbConnection.Close();

            return reader["Name"];
        }

    }
}