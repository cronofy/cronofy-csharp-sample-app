using System;
using System.Data.SQLite;

namespace CronofyCSharpSampleApp
{
    public interface ITableRowModel
    {
        ITableRowModel Initialize(SQLiteDataReader row);
        ITableRowModel Initialize(object obj);
    }
}
