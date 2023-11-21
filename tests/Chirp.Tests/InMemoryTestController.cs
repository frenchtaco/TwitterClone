using Microsoft.Data.Sqlite;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using SQLitePCL;

public class InMemoryTestController //Inspired by https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database
{

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DatabaseContext> _contextOptions;

    public InMemoryTestController()
    {
        _connection = new SqliteConnection("filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(_connection).Options;

        using var context = new DatabaseContext(_contextOptions);

        context.Database.EnsureCreated(); //Can use template below to create a view for later queries.
        /*if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"
                CREATE VIEW AllCheeps AS
                SELECT Text
                FROM Cheeps;";
            viewCommand.ExecuteNonQuery();
        }*/

        context.AddRange(
            new Cheep { CheepId = 1, Author = new Author() { Email = "author1@id.com", UserName = "AuthorName1" }, Text = "First cheep", TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, Author = new Author() { Email = "author2@id.com", UserName = "AuthorName2" }, Text = "Second cheep", TimeStamp = DateTime.Now },
            new Cheep { CheepId = 3, Author = new Author() { Email = "author3@id.com", UserName = "AuthorName3" }, Text = "Third cheep", TimeStamp = DateTime.Now });
        context.SaveChanges();

    }

    DatabaseContext CreateContext() => new DatabaseContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    /*
        [Fact]
        public async void GetAllCheeps()
        {
            using var context = CreateContext();
            var repository = new CheepRepository(context, new AuthorRepository(context), null);

            IEnumerable<Cheep> cheeps = await repository.GetAllCheeps();

            Assert.Collection(
                cheeps.Reverse(),
                cheep => Assert.Equal("First cheep", cheep.Text),
                cheep => Assert.Equal("Second cheep", cheep.Text),
                cheep => Assert.Equal("Third cheep", cheep.Text));
        }
        */
}
