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

        for (int i = 0; i < 12; i++)
        {
            Author newAuthor = _authorRepository.CreateAuthor(namesArray[i], $"author{i}@id.com");

            Authors.Add(newAuthor);
            _dbContext.Authors.Add(newAuthor);
        }
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