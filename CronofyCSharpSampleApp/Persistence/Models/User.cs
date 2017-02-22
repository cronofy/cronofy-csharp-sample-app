﻿using System;
using System.Data.SQLite;

namespace CronofyCSharpSampleApp.Persistence.Models
{
    public class User : ITableRowModel
    {
        public string CronofyUID { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public User() { }

        public User(string cronofyUID, string accessToken, string refreshToken)
        {
            CronofyUID = cronofyUID;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public ITableRowModel Initialize(SQLiteDataReader row)
        {
            CronofyUID = row.GetString(0);
            AccessToken = row.GetString(1);
            RefreshToken = row.GetString(2);

            return this;
        }

        public ITableRowModel Initialize(object obj)
        {
            return this;
        }
    }
}
