using Chirp.CDTO;
using Chirp.Infrastructure;
using Chirp.Interfaces;
using DBContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CT;

public class CreateTestAndGetTests
{
    private readonly InMemoryTestController DB;
    private readonly DatabaseContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly IServiceProvider _serviceProvider;
    private List<Author> Authors { get; set; } = null!;
    
    public CreateTestAndGetTests()
    {
        DB = new InMemoryTestController();
        _dbContext = DB.GetDatabaseContext();
        _serviceProvider = DB.ServiceProvider;

        _authorRepository  = _serviceProvider.GetRequiredService<IAuthorRepository>();
        _cheepRepository   = _serviceProvider.GetRequiredService<ICheepRepository>();

        Authors = new List<Author>();

        InitAuthors();
    }

    private void InitAuthors()
    {
        for (int i = 0; i < 64; i++)
        {
            Author newAuthor = _authorRepository.CreateAuthor($"Author{i}", $"author{i}@id.com");
            Authors.Add(newAuthor);
            _dbContext.Authors.Add(newAuthor);
        }
    }

    [Fact]
    public void CreateAuthorsTest()
    {
        Assert.Equal(64, _dbContext.Authors.Count());
    }

    [Fact]
    public async void CreateCheepsTest()
    {
        // 01. Create Cheeps:
        for (int i = 0; i < 64; i++)
        {
            CheepDTO newCheepDTO = new CheepDTO($"Cheep{i}", Authors[i].UserName);
            await _cheepRepository.CreateCheep(newCheepDTO);
        }

        // 02. Confirm the amount of Cheeps and check their associated Authors have a connection to them.
        Assert.Equal(64, _dbContext.Cheeps.Count());

        foreach (Author author in _dbContext.Authors)
        {
            Assert.NotEmpty(author.Cheeps);
        }

        // 03. Confirm that the Cheeps are neither empty nor null:
        foreach (Cheep cheep in _dbContext.Cheeps)
        {
            Assert.False(string.IsNullOrEmpty(cheep.Text));
        }
    }

    [Fact]
    public async void GetTop4CheepsFromAuthorTest()
    {
        Author author = _authorRepository.CreateAuthor("Author0", "ao@itu.dk") ?? throw new Exception("Unable to successfully create a new Author");
        await _dbContext.SaveChangesAsync();

        for (int i = 0; i < 6; i++)
        {
            await Task.Delay(1000);
            CheepDTO newCheepDTO = new CheepDTO($"Cheep{i}", author.UserName);
            await _cheepRepository.CreateCheep(newCheepDTO);
        }

        Console.WriteLine("Number of Cheeps: " + _dbContext.Cheeps.Count());

        IEnumerable<Cheep> _top4Cheeps = await _cheepRepository.GetTop4FromAuthor("Author0");
        List<Cheep> top4Cheeps = _top4Cheeps.ToList();

        Assert.Equal(4, _top4Cheeps.Count());
        Assert.Equal("Cheep5", top4Cheeps[0].Text);
        Assert.Equal("Cheep4", top4Cheeps[1].Text);
        Assert.Equal("Cheep3", top4Cheeps[2].Text);
        Assert.Equal("Cheep2", top4Cheeps[3].Text);
    }


}