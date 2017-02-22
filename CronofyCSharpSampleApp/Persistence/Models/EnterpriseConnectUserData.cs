using System;
using System.Data.SQLite;

namespace CronofyCSharpSampleApp.Persistence.Models
{
    public class EnterpriseConnectUserData : ITableRowModel
    {
        public string CronofyUID { get; private set; }
        public string Email { get; private set; }
        public ConnectedStatus Status { get; private set; }

        public EnterpriseConnectUserData() { }

        public EnterpriseConnectUserData(string cronofyUID, string email, ConnectedStatus status)
        {
            CronofyUID = cronofyUID;
            Email = email;
            Status = status;
        }

        public ITableRowModel Initialize(SQLiteDataReader row)
        {
            CronofyUID = row.IsDBNull(0) ? String.Empty : row.GetString(0);
            Email = row.GetString(1);
            Status = (ConnectedStatus)row.GetInt32(2);

            return this;
        }

        public ITableRowModel Initialize(Mono.Data.Sqlite.SqliteDataReader row)
        {
            CronofyUID = row.IsDBNull(0) ? String.Empty : row.GetString(0);
            Email = row.GetString(1);
            Status = (ConnectedStatus)row.GetInt32(2);

            return this;
        }

        public enum ConnectedStatus
        {
            Pending = 0,
            Linked = 1,
            Failed = 2
        }
    }
}
