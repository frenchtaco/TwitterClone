using Chirp.Models;
using Chirp.ADTO;
using Chirp.FDTO;

namespace Chirp.Interfaces;

public interface IAuthorRepository
{
    public Author CreateAuthor(string authorName, string email);
    public Task<Author> GetAuthorByName(string authorName); 
    public Task<IEnumerable<Author>> GetAllAuthors();
    public Task<IEnumerable<Author>> GetAuthorFollowers(string authorName);
    public Task<IEnumerable<Author>> GetAuthorFollowing(string authorName);
    public Task<bool> ForgetAuthor(string authorName);
    public Task Follow(FollowersDTO followersDTO);
    public Task Unfollow(FollowersDTO followersDTO);
}