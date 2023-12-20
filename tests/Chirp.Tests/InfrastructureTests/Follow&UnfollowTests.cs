using Chirp.FDTO;
using Chirp.Interfaces;
using DBContext;
using Microsoft.Extensions.DependencyInjection;

namespace FAU;

public class FollowAndUnFollowTest
{
    private readonly InMemoryTestController DB;
    private readonly DatabaseContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthorRepository _authorRepository;
    private List<Author> Authors { get; set; } = null!;

    public FollowAndUnFollowTest()
    {
        DB = new InMemoryTestController();
        _dbContext = DB.GetDatabaseContext();
        _serviceProvider = DB.ServiceProvider;

        _authorRepository  = _serviceProvider.GetRequiredService<IAuthorRepository>();

        Authors = new List<Author>();

        for (int i = 0; i < 12; i++)
        {
            Author newAuthor = _authorRepository.CreateAuthor($"Author{i}", $"author{i}@id.com");

            Authors.Add(newAuthor);
            _dbContext.Authors.Add(newAuthor);
        }

        _dbContext.SaveChanges();
    }

    [Fact]
    public async void FollowUnfollowTest()
    {
        Author targetAuthor = await _authorRepository.GetAuthorByName("Author10");
        Author authorToFollow = await _authorRepository.GetAuthorByName("Author11");

        FollowersDTO followerDTO = new FollowersDTO(targetAuthor.UserName, authorToFollow.UserName);
        await _authorRepository.Follow(followerDTO);

        IEnumerable<Author> following = await _authorRepository.GetAuthorFollowing(targetAuthor.UserName);
        IEnumerable<Author> followers = await _authorRepository.GetAuthorFollowers(authorToFollow.UserName);

        Assert.Contains(authorToFollow, following);
        Assert.Contains(targetAuthor, followers);

        await _authorRepository.Unfollow(followerDTO);

        following = await _authorRepository.GetAuthorFollowing(targetAuthor.UserName);
        followers = await _authorRepository.GetAuthorFollowers(authorToFollow.UserName);

        Assert.DoesNotContain(authorToFollow, following);
        Assert.DoesNotContain(targetAuthor, followers);
    }

    // [Fact]
    // public async void FollowTest()
    // {
    //     // 01. Follow an Author
    //     Author a1 = await _authorRepository.GetAuthorByName("Author1");
    //     Author a2 = await _authorRepository.GetAuthorByName("Author2");
    //     Author a3 = await _authorRepository.GetAuthorByName("Author3");
    //     Author a4 = await _authorRepository.GetAuthorByName("Author4");
    //     Author a5 = await _authorRepository.GetAuthorByName("Author5");
    //     Author a6 = await _authorRepository.GetAuthorByName("Author6");

    //     // 02. Pack Authors into a List
    //     List<Author> authorList = new List<Author> { a1, a2, a3, a4, a5, a6 };

    //     // 03. Make them all Follow the listed target Author:
    //     Author targetAuthor = await _authorRepository.GetAuthorByName("Author7");

    //     foreach (Author author in authorList) 
    //     {
    //         FollowersDTO followerDTO = new FollowersDTO(targetAuthor.UserName, author.UserName);
    //         await _authorRepository.Follow(followerDTO);
    //     }

    //     // 04. Get these new followers and confirm the intended amount:
    //     IEnumerable<Author> followers = await _authorRepository.GetAuthorFollowers(targetAuthor.UserName);

    //     Assert.Equal(authorList.Count, followers.Count());
    // } 
}