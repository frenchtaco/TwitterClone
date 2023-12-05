using Microsoft.Data.Sqlite;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.FDTO;
using Chirp.CDTO;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
[SuppressMessage("xUnit", "xUnit1013:Public method 'Dispose'...")] // Suppress Warning about Dispose() not being a Fact
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

        context.Database.EnsureCreated();

        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        List<Author> authors = new();
        for (int i = 1; i <= 64; i++)
        {
            Author author = new Author
                            {
                                UserName = $"AuthorName{i}",
                                Email = $"author{i}@id.com",
                                Cheeps = new List<Cheep>(),
                                Followers = new HashSet<Author>(),
                                Following = new HashSet<Author>()
                            };
            authors.Add(author);
            context.Authors.Add(author);
        }

        context.Cheeps.AddRange(Enumerable.Range(1, 64).Select(i =>
            new Cheep
            {
                CheepId = i,
                Author = authors[i-1],
                Text = $"{i}. cheep",
                TimeStamp = DateTime.UtcNow
            }));
        context.SaveChanges();

    }

    DatabaseContext CreateContext() => new DatabaseContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    /// <summary>
    ///                                                       ///////////////////////////////////
    ///                                                             Cheep Repository Tests
    ///                                                       ///////////////////////////////////
    /// </summary>

    [Fact]
    public async void GetCheeps()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        IEnumerable<Cheep> _cheeps = await cheepRepository.GetCheeps(0);
        List<Cheep> cheeps = _cheeps.ToList();

        Assert.Equal(32, _cheeps.Count());
        Assert.Equal("64. cheep", cheeps[0].Text);
        Assert.Equal("33. cheep", cheeps[31].Text);
    }

    [Fact]
    public async void GetTotalNumberOfCheeps()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        var numberOfCheeps = await cheepRepository.GetTotalNumberOfCheeps();

        Assert.Equal(64, numberOfCheeps);
    }

    [Fact]
    public async void GetCheepsFromAuthor()
    {
        using var context = CreateContext();

        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        IEnumerable<Cheep> _cheeps = await cheepRepository.GetCheepsFromAuthor("AuthorName10", 0);
        List<Cheep> cheeps = _cheeps.ToList();

        IEnumerable<Cheep> _cheepsPageTwo = await cheepRepository.GetCheepsFromAuthor("AuthorName10", 1);
        List<Cheep> cheepsPageTwo = _cheepsPageTwo.ToList();

        Assert.Single(cheeps);
        Assert.Equal("10. cheep", cheeps.First().Text);

        Assert.Empty(cheepsPageTwo);
    }

    [Fact]
    public async void GetAllCheepsFromAuthor()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        IEnumerable<Cheep> _cheeps = await cheepRepository.GetAllCheepsFromAuthor("AuthorName10");
        List<Cheep> cheeps = _cheeps.ToList();

        Assert.Single(cheeps);
        Assert.Equal("10. cheep", cheeps.First().Text);
    }

    [Fact]
    public async void GetTop4FromAuthor()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        for (int i = 0; i < 4; i++)
        {
            Thread.Sleep(1000);
            CheepDTO newCheep = new CheepDTO($"3. cheep{i+2}", "AuthorName3");
            await cheepRepository.CreateCheep(newCheep);
        }

        IEnumerable<Cheep> _allCheeps = await cheepRepository.GetAllCheepsFromAuthor("AuthorName3");
        Assert.Equal(5, _allCheeps.Count());

        IEnumerable<Cheep> _top4Cheeps = await cheepRepository.GetTop4FromAuthor("AuthorName3");
        List<Cheep> top4Cheeps = _top4Cheeps.ToList();

        Assert.Equal(4, _top4Cheeps.Count());
        Assert.Equal("3. cheep5", top4Cheeps[0].Text);
        Assert.Equal("3. cheep4", top4Cheeps[1].Text);
        Assert.Equal("3. cheep3", top4Cheeps[2].Text);
        Assert.Equal("3. cheep2", top4Cheeps[3].Text);
    }

    [Fact]
    public async void CreateCheep()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        var numberOfCheeps = await cheepRepository.GetTotalNumberOfCheeps();
        Assert.Equal(64, numberOfCheeps);

        CheepDTO newCheep = new CheepDTO("7. cheep2", "AuthorName7");
        await cheepRepository.CreateCheep(newCheep);

        numberOfCheeps = await cheepRepository.GetTotalNumberOfCheeps();
        Assert.Equal(64, numberOfCheeps);
    }

    /// <summary>
    ///                                                       ///////////////////////////////////
    ///                                                             Author Repository Tests
    ///                                                       ///////////////////////////////////
    /// </summary>

    [Fact]
    public async void GetAllAuthors()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        IEnumerable<Author> _authors = await authorRepository.GetAllAuthors();

        Assert.Equal(64, _authors.Count());
    }

    [Fact]
    public async void GetAuthorByName()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        Author targetAuthor = await authorRepository.GetAuthorByName("AuthorName19");

        Assert.Equal("AuthorName19", targetAuthor.UserName);
    }

    [Fact]
    public async void Following()
    {
        using var context = CreateContext();
        var authorRepository = new AuthorRepository(null, context);
        var cheepRepository = new CheepRepository(context, authorRepository, null, null);

        Author targetAuthor = await authorRepository.GetAuthorByName("AuthorName12");
        Author authorToFollow = await authorRepository.GetAuthorByName("AuthorName13");
        FollowersDTO followerDTO = new FollowersDTO(targetAuthor.UserName, authorToFollow.UserName);
        
        await authorRepository.Follow(followerDTO);
        IEnumerable<Author> following = await authorRepository.GetAuthorFollowing(targetAuthor.UserName);
        IEnumerable<Author> followers = await authorRepository.GetAuthorFollowers(authorToFollow.UserName);
        Assert.Contains(authorToFollow, following);
        Assert.Contains(targetAuthor, followers);

        await authorRepository.Unfollow(followerDTO);
        following = await authorRepository.GetAuthorFollowing(targetAuthor.UserName);
        followers = await authorRepository.GetAuthorFollowers(authorToFollow.UserName);
        Assert.DoesNotContain(authorToFollow, following);
        Assert.DoesNotContain(targetAuthor, followers);
    }
}
