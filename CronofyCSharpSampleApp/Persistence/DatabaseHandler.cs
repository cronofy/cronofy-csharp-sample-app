﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;

namespace CronofyCSharpSampleApp
{
    public static class DatabaseHandler
    {
        private static bool _mac = false;
        private static string _connectionString;


        static DatabaseHandler()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var pathToDatabase = Path.Combine(currentDirectory, "Persistence/cronofy_sample_app.db");

            _connectionString = String.Format("Data Source={0};Version=3;", pathToDatabase);
            _mac = Environment.OSVersion.Platform != PlatformID.WinCE 
                              && Environment.OSVersion.Platform != PlatformID.Win32S 
                              && Environment.OSVersion.Platform != PlatformID.Win32NT
                              && Environment.OSVersion.Platform != PlatformID.Win32Windows;

            if (!File.Exists(pathToDatabase))
            {

                if (_mac)
                {
                    Mono.Data.Sqlite.SqliteConnection.CreateFile(pathToDatabase);
                }
                else {
                    SQLiteConnection.CreateFile(pathToDatabase);
                }

                ExecuteNonQuery("CREATE TABLE ChannelData (Id INTEGER PRIMARY KEY AUTOINCREMENT, ChannelId ntext, Record ntext, OccurredOn datetime2)");
                ExecuteNonQuery("CREATE TABLE UserCredentials (UserID INTEGER PRIMARY KEY AUTOINCREMENT, CronofyUID ntext, AccessToken ntext, RefreshToken ntext, ServiceAccount bit NOT NULL)");
                ExecuteNonQuery("CREATE TABLE EnterpriseConnectUserData (Id INTEGER PRIMARY KEY AUTOINCREMENT, CronofyUID ntext, OwnedBy ntext, Email ntext, status integer)");
            }
        }

        public static IEnumerable<T> Many<T>(string sql, IDictionary<string, object> parameters) where T : ITableRowModel, new()
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;

                    foreach(var parameter in parameters)
                    {
                        cmd.Parameters.Add(CreateParameter(cmd, "@" + parameter.Key, parameter.Value));
                    }

                    var reader = cmd.ExecuteReader();

                    var rows = new List<T>();

                    while (reader.Read())
                    {
                        rows.Add((T)(new T().Initialize(reader)));
                    }

                    return rows;
                }
            }
        }

        public static T Get<T>(string sql, IDictionary<string, object> parameters) where T : ITableRowModel, new()
        {
            return Many<T>(sql, parameters).FirstOrDefault();
        }

        public static object Scalar(string sql, IDictionary<string, object> parameters)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;

                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(CreateParameter(cmd, "@" + parameter.Key, parameter.Value));
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

        public static void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, new Dictionary<string, object>());
        }

        public static void ExecuteNonQuery(string sql, IDictionary<string, object> parameters)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;

                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(CreateParameter(cmd, "@" + parameter.Key, parameter.Value));
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DbConnection CreateConnection()
        {
            if (_mac)
            {
                return new Mono.Data.Sqlite.SqliteConnection(_connectionString);
            }
            else
            {
                return new SQLiteConnection(_connectionString);
            }
        }

        public static DbParameter CreateParameter(DbCommand cmd, string key, object value)
        {
            var parameter = cmd.CreateParameter();

            parameter.ParameterName = key;
            parameter.Value = value;

            return parameter;
        }
    }
}
