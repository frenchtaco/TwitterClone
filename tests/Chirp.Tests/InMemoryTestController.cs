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
    public void SeedDB() 
    {
        _databaseContext.AddRange(
            new Cheep { CheepId = 1, Author = new Author() { Email = "author1@id.com", UserName = "AuthorName1" }, Text = "First cheep", TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, Author = new Author() { Email = "author2@id.com", UserName = "AuthorName2" }, Text = "Second cheep", TimeStamp = DateTime.Now },
            new Cheep { CheepId = 3, Author = new Author() { Email = "author3@id.com", UserName = "AuthorName3" }, Text = "Third cheep", TimeStamp = DateTime.Now });
        _databaseContext.SaveChanges();
    }
}
