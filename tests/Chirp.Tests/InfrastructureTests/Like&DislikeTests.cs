using Chirp.CDTO;
using Chirp.Infrastructure;
using Chirp.Interfaces;
using Chirp.ODTO;
using DBContext;
using Enums.ACO;
using Microsoft.Extensions.DependencyInjection;

namespace LAD;

public class LikeAndDislikeTest
{
    private readonly InMemoryTestController DB;
    private readonly DatabaseContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly ILikeDisRepository _likeDisRepository;
    private readonly IServiceProvider _serviceProvider;

    private Author Author1 { get; set; } = null!;
    private Author Author2 { get; set; } = null!;

    public LikeAndDislikeTest()
    {
        DB = new InMemoryTestController();
        _dbContext = DB.GetDatabaseContext();
        _serviceProvider = DB.ServiceProvider;

        _authorRepository  = _serviceProvider.GetRequiredService<IAuthorRepository>();
        _cheepRepository   = _serviceProvider.GetRequiredService<ICheepRepository>();
        _likeDisRepository = _serviceProvider.GetRequiredService<ILikeDisRepository>();


    }

    [Fact]
    public async void LikeDislikeTest()
    {
        // 01. Initiation:
        Author1 = _authorRepository.CreateAuthor("AdamWest", "js@itu.dk");
        Author2 = _authorRepository.CreateAuthor("DarthMaul", "dm@itu.dk");

        _dbContext.Authors.Add(Author1);
        _dbContext.Authors.Add(Author2);

        _dbContext.SaveChanges();

        for (int i = 0; i < 32; i++)
        {
            CheepDTO newCheep = new CheepDTO($"Cheep Nr.{i+1}", "AdamWest");
            await _cheepRepository.CreateCheep(newCheep);
        }

        // 02. 
        bool IsLike = true;
        string Author1UserName = Author1.UserName;
        string Author2UserName = Author2.UserName;

        IEnumerable<Cheep> Top4Cheeps = await _cheepRepository.GetTop4FromAuthor(Author1UserName);
        List<Cheep> Top4CheepsList = Top4Cheeps.ToList();
        
        foreach (Cheep cheeps in Top4CheepsList)
        {
            await _cheepRepository.GiveOpinionOfCheep(IsLike, cheeps.CheepId, Author2UserName);
        }

        Dictionary<int, CheepOpinionDTO> AuthorOpinionOfCheeps = new Dictionary<int, CheepOpinionDTO>();

        foreach (Cheep cheeps in Top4CheepsList)
        {
            CheepOpinionDTO opinion = await _likeDisRepository.GetAuthorCheepOpinion(cheeps.CheepId, Author2UserName);
            AuthorOpinionOfCheeps.Add(cheeps.CheepId, opinion);
        }

        int totalNumLikes = 0, totalNumDislikes = 0;

        foreach (KeyValuePair<int, CheepOpinionDTO> entry in AuthorOpinionOfCheeps)
        {
            CheepOpinionDTO value = entry.Value;

            int numLikes = value.NumLikes;
            int numDislikes = value.NumDislikes;
            AuthorCheepOpinion aco = value.AuthorCheepOpinion;
            
            Assert.Equal(AuthorCheepOpinion.LIKES, value.AuthorCheepOpinion);
            Assert.Equal(1, numLikes);
            Assert.Equal(0, numDislikes);

            totalNumLikes += numLikes;
            totalNumDislikes += numDislikes;
        }

        Assert.Equal(4, totalNumLikes);
        Assert.Equal(0, totalNumDislikes);
    }
}