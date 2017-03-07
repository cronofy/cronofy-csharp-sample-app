using System.Data.Common;

namespace CronofyCSharpSampleApp
{
    public interface ITableRowModel
    {
        ITableRowModel Initialize(DbDataReader row);
    }
}
