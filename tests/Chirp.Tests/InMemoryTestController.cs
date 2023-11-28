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

        context.AddRange(Enumerable.Range(1, 64).Select(i =>
            new Cheep
            {
                CheepId = i,
                Author = new Author()
                {
                    Email = $"author{i}@id.com",
                    UserName = $"AuthorName{i}"
                },
                Text = $"{i}. cheep",
                TimeStamp = DateTime.Now.AddSeconds(i)
            }));
        context.SaveChanges();

    }

    DatabaseContext CreateContext() => new DatabaseContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    
        [Fact]
        public async void GetAllCheeps()
        {
            using var context = CreateContext();
            var repository = new CheepRepository(context, new AuthorRepository(null, context), null);

            IEnumerable<Cheep> cheeps = await repository.GetAllCheeps();

            Assert.Equal(64, cheeps.Count());
        }

        [Fact]
        public async void GetCheeps()
        {
            using var context = CreateContext();
            var repository = new CheepRepository(context, new AuthorRepository(null, context), null);

            IEnumerable<Cheep> _cheeps = await repository.GetCheeps(0);
            List<Cheep> cheeps = _cheeps.ToList();

            Assert.Equal(32, _cheeps.Count());
            Assert.Equal("64. cheep", cheeps[0].Text);
            Assert.Equal("33. cheep", cheeps[31].Text);
        }

        [Fact]
        public async void Follow()
        {
            using var context = CreateContext();
            var repository = new AuthorRepository(null, context), null;

            Author currentAuthor = repository.GetAuthorByName(AuthorName12);
            
        }
}
