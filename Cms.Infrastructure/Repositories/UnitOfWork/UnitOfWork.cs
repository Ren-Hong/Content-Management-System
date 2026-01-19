using Cms.Infrastructure.Repositories.UnitOfWork;
using System.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;

    public IDbTransaction? Transaction { get; private set; }

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public void BeginTransaction()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        Transaction = _connection.BeginTransaction(); // 這段會給 Tx 交易物建
    }

    public void Commit()
    {
        try
        {
            Transaction?.Commit();
        }
        finally
        {
            Transaction?.Dispose();
            Transaction = null;
        }
    }

    public void Rollback()
    {
        try
        {
            Transaction?.Rollback();
        }
        finally
        {
            Transaction?.Dispose();
            Transaction = null;
        }
    }
}
