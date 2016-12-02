using System;
using Mono.Data.Sqlite;

namespace CronofyCSharpSampleApp
{
	public interface ITableRowModel
	{
		ITableRowModel Initialize(SqliteDataReader row);
		ITableRowModel Initialize(object obj);
	}
}
