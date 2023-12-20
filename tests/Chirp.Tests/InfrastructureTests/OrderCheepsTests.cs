using Chirp.CDTO;
using Chirp.Infrastructure;
using Chirp.Interfaces;
using DBContext;

namespace OCT;

public class OrderCheepsByTest 
{
    private readonly InMemoryTestController DB;
    private readonly DatabaseContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly ILikeDisRepository _likeDisRepository;
    private List<Author> Authors { get; set; } = null!;

    public OrderCheepsByTest()
    {
        DB = new InMemoryTestController();
        _dbContext = DB.GetDatabaseContext();

        _authorRepository  = new AuthorRepository(_dbContext);
        _likeDisRepository = new LikeDisRepository(_dbContext, _authorRepository);
        _cheepRepository   = new CheepRepository(_dbContext, _authorRepository, _likeDisRepository);

        Authors = new List<Author>();

        InitGlobalVariables();
    }

    
    private async void InitGlobalVariables()
    {
        for (int i = 0; i < namesArray.Length; i++)
        {
            Author newAuthor = _authorRepository.CreateAuthor(namesArray[i], $"{namesArray[i]}{i}@id.com");

            Authors.Add(newAuthor);
            _dbContext.Authors.Add(newAuthor);
        }
        _dbContext.SaveChanges();
    }

    [Fact]
    public async void OrderByLikesAndNameTest()
    {
        // 01. Initiate the creation of Cheeps:
        for (int i = 0; i < namesArray.Length; i++)
        {
            CheepDTO newCheepDTO = new CheepDTO($"Cheep{i}", Authors[i].UserName);
            await _cheepRepository.CreateCheep(newCheepDTO);
        }

        // 02. Partition a gradually decrementing number of Likes amongst the Cheeps
        int likesCounter = 1;

        foreach (Cheep cheep in _dbContext.Cheeps)
        {
            CheepLikeDis cld = await _likeDisRepository.GetCheepLikeDis(cheep.CheepId) ?? throw new Exception("Could not find the 'CheepLikeDisSchema'");

            for (int i = 0; i < likesCounter; i++)
            {
                cld.Likes.Add(Authors[i]);
            }
            likesCounter++;
        }

        _dbContext.SaveChanges();

        // 03. Sort by number of Likes:
        IEnumerable<Cheep> cheepsSortedByLikes = await _cheepRepository.GetCheeps(0, "liked");
        List<Cheep> csbl = cheepsSortedByLikes.ToList();

        Cheep topCheepByLikes = csbl.FirstOrDefault() ?? throw new Exception("[OrderCheepsTest.cs] Cheep could NOT be located");

        Author targetAuthor = await _authorRepository.GetAuthorByName("Leo");

        Assert.Equal(targetAuthor, topCheepByLikes.Author);


        // 04. Sort by Cheep's Author Names:
        IEnumerable<Cheep> cheepsSortedByName = await _cheepRepository.GetCheeps(0, "name");
        List<Cheep> csbn = cheepsSortedByName.ToList();

        Cheep topCheepByName = csbn.FirstOrDefault() ?? throw new Exception("[OrderCheepsTest.cs] Cheep could NOT be located");

        targetAuthor = await _authorRepository.GetAuthorByName("Alice");

        Assert.Equal(targetAuthor, topCheepByName.Author);
    }

    string[] namesArray = {
        "Alice",
        "Bob",
        "Carol",
        "David",
        "Emily",
        "Frank",
        "Grace",
        "Henry",
        "Iris",
        "Jack",
        "Kate",
        "Leo"
    };
}