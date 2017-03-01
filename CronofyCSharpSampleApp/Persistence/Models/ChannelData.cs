using System;
using System.Data.SQLite;

namespace CronofyCSharpSampleApp.Persistence.Models
{
    public class ChannelData : ITableRowModel
    {
        public string ChannelId { get; private set; }
        public string Record { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public ChannelData() { }

        public ChannelData(string channelId, string record, DateTime occurredOn)
        {
            ChannelId = channelId;
            Record = record;
            OccurredOn = occurredOn;
        }

        public ITableRowModel Initialize(SQLiteDataReader row)
        {
            ChannelId = row.GetString(0);
            Record = row.GetString(1);
            OccurredOn = row.GetDateTime(2);

            return this;
        }

        public ITableRowModel Initialize(Mono.Data.Sqlite.SqliteDataReader row)
        {
            ChannelId = row.GetString(0);
            Record = row.GetString(1);
            OccurredOn = row.GetDateTime(2);

            return this;
        }
    }
}
