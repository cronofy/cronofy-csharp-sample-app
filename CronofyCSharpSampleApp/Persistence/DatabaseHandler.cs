using System;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CronofyCSharpSampleApp
{
	public static class DatabaseHandler
	{
		private static bool _initialized = false;
		private static string _connectionString;


		public static void Initialize()
		{
			var currentDirectory = Environment.CurrentDirectory;
			var pathToDatabase = Path.Combine(currentDirectory, "/Persistence/cronofy_sample_app.db");

			_connectionString = String.Format("Data Source={0};Version=3;", pathToDatabase);
			_initialized = true;

			if (!File.Exists(pathToDatabase))
			{
				SqliteConnection.CreateFile(pathToDatabase);

                ExecuteNonQuery("CREATE TABLE ChannelData (Id INTEGER PRIMARY KEY AUTOINCREMENT, ChannelId ntext, Record ntext, OccurredOn datetime2)");
                ExecuteNonQuery("CREATE TABLE Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, CronofyUID ntext, AccessToken ntext, RefreshToken ntext, ServiceAccount bit NOT NULL)");
                ExecuteNonQuery("CREATE TABLE EnterpriseConnectUserData (Id INTEGER PRIMARY KEY AUTOINCREMENT, CronofyUID ntext, OwnedBy ntext, Email ntext, status integer)");
			}
		}

		public static IEnumerable<T> Many<T>(string sql) where T : ITableRowModel, new()
		{
			if (!_initialized)
				Initialize();

			using (var conn = new SqliteConnection(_connectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = sql;
					cmd.CommandType = System.Data.CommandType.Text;
					SqliteDataReader reader = cmd.ExecuteReader();

					var rows = new List<T>();

					while (reader.Read())
					{
						rows.Add((T)(new T().Initialize(reader)));
					}

					return rows;
				}
			}
		}

		public static T Get<T>(string sql) where T : ITableRowModel, new()
		{
			return Many<T>(sql).First();
		}

		public static object Scalar(string sql)
		{
			if (!_initialized)
				Initialize();
            
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    return cmd.ExecuteScalar();
                }
            }
		}

		public static void ExecuteNonQuery(string sql)
		{
			if (!_initialized)
				Initialize();

			using (var conn = new SqliteConnection(_connectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = sql;
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
