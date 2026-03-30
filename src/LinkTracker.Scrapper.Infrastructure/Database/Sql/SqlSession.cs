using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Database.Sql;

public class SqlSession
{
    public NpgsqlConnection?  Connection { get; set; }
    public NpgsqlTransaction? Transaction { get; set; }
    public bool HasTransaction => Connection != null && Transaction != null;

    public void Clear()
    {
        Connection =  null;
        Transaction = null;
    }
}