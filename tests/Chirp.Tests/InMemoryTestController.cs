using Microsoft.Data.Sqlite;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using SQLitePCL;
using Chirp.CDTO;

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

        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null);

        context.Authors.AddRange(Enumerable.Range(1, 64).Select(i =>
            new Author
            {
                UserName = $"AuthorName{i}",
                Email = $"author{i}@id.com",
                Cheeps = new List<Cheep>(),
                Followers = new HashSet<Author>(),
                Following = new HashSet<Author>()
            }));
        context.SaveChanges();

        /*context.Cheeps.AddRange(Enumerable.Range(1, 64).Select(async i =>
            new Cheep
            {
                CheepId = i,
                Author = await repository.GetAuthorByName($"AuthorName{i}"),
                Text = $"{i}. cheep",
                TimeStamp = DateTime.Now.AddSeconds(i)
            }));*/
        context.SaveChanges();

    }

    DatabaseContext CreateContext() => new DatabaseContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    public async Task InitializeDB(CheepRepository cheepRepository)
    {
        for (int i = 1; i <= 64; i++)
        {
            CheepDTO cheep = new CheepDTO($"{i}. cheep", $"AuthorName{i}");
            await cheepRepository.CreateCheep(cheep);
        }
    }

    [Fact]
    public async void GetAllCheeps()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null);

        await InitializeDB(cheepRepository);
        IEnumerable<Cheep> cheeps = await cheepRepository.GetAllCheeps();

        Assert.Equal(64, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null);

        IEnumerable<Cheep> _cheeps = await cheepRepository.GetCheeps(0);
        List<Cheep> cheeps = _cheeps.ToList();

        Assert.Equal(32, _cheeps.Count());
        Assert.Equal("64. cheep", cheeps[0].Text);
        Assert.Equal("33. cheep", cheeps[31].Text);
    }

    [Fact]
    public async void TestFollowing()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null);

        Author currentAuthor = await authorRepository.GetAuthorByName("AuthorName12");
        Author targetAuthor = await authorRepository.GetAuthorByName("AuthorName13");

        //FollowersDTO followerDTO = new FollowersDTO(targetAuthor.UserName, author13.UserName);


    }
}
