using Microsoft.Data.Sqlite;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using SQLitePCL;

public class InMemoryTestController //Inspired by https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database
{

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DatabaseContext> _contextOptions;
    private readonly DatabaseContext _databaseContext;

    public InMemoryTestController()
    {
        _connection      = new SqliteConnection("filename=:memory:");
        _connection.Open();

        _contextOptions  = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(_connection).Options;
        _databaseContext = new DatabaseContext(_contextOptions);

        _databaseContext.Database.EnsureCreated();
    }
    public DatabaseContext GetDatabaseContext() { return _databaseContext; }
    public void Dispose() => _connection.Dispose();
}
