using System;
using Mono.Data.Sqlite;

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

		public ITableRowModel Initialize(SqliteDataReader row)
		{
			ChannelId = row.GetString(0);
			Record = row.GetString(1);
			OccurredOn = DateTime.Parse(row.GetString(2));

			return this;
		}

		public ITableRowModel Initialize(object obj)
		{
			return this;
		}
	}
}
