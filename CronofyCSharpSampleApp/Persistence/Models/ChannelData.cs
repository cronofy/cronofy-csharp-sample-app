using System;
using Mono.Data.Sqlite;

namespace CronofyCSharpSampleApp
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

		public ITableRowModel Initialize(SqliteDataReader row)
		{
			ChannelId = row.GetString(1);
			Record = row.GetString(2);
			OccurredOn = row.GetDateTime(3);

			return this;
		}

		public ITableRowModel Initialize(object obj)
		{
			return this;
		}
	}
}
