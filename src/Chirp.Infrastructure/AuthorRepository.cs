using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.ADTO;
using Microsoft.EntityFrameworkCore;
using Chirp.FDTO;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ILogger<AuthorRepository> _logger;
    private readonly DatabaseContext _context;

    public AuthorRepository(ILogger<AuthorRepository> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IEnumerable<Author>> GetAllAuthors()
    {
        return await _context.Authors
            .Include(a => a.Following)
            .Include(a => a.Followers)
            .ToListAsync();
    }

    public async Task<bool> ForgetAuthor(string authorName)
    {
        try
        {

            // [TODO] Change it to proper delete and instead make "Account Recovery"
            var author = await GetAuthorByName(authorName);

            author.UserName = "NULL";
            author.Cheeps.Clear();
            author.IsForgotten = true;

            await _context.SaveChangesAsync();

            return true;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Author> GetAuthorByName(string authorName)
    {
        return await _context.Authors
            .Include(a => a.Following)
            .Include(a => a.Followers)
            .Where(a => a.UserName == authorName)
            .FirstOrDefaultAsync() ?? throw new Exception("Author could not be located");
    }

    public async Task Follow(FollowersDTO followersDTO)
    {
        try
        {
            var targetAuthor = await GetAuthorByName(followersDTO.TargetAuthor);
            var authorToFollow = await GetAuthorByName(followersDTO.FollowAuthor);

            if(!targetAuthor.Following.Contains(authorToFollow)) // additional check that !(authorToFollow.Followers.Contains(targetAuthor))
            {                
                targetAuthor.Following.Add(authorToFollow);
                authorToFollow.Followers.Add(targetAuthor);
            } 

            await _context.SaveChangesAsync();
        } 
        catch(Exception ex)
        {
            throw new Exception($"Exception: {ex.Message}");
        }
    }

    public async Task Unfollow(FollowersDTO followersDTO)
    {
        try
        {  
            var targetAuthor = await GetAuthorByName(followersDTO.TargetAuthor);
            var authorToUnfollow = await GetAuthorByName(followersDTO.FollowAuthor);

            if(targetAuthor.Following.Contains(authorToUnfollow))
            {
                targetAuthor.Following.Remove(authorToUnfollow);
                authorToUnfollow.Followers.Remove(targetAuthor);
            } 

            await _context.SaveChangesAsync();
        } 
        catch(Exception ex)
        {
            throw new Exception($"Exception: {ex.Message}");
        }
    }


    public async Task<IEnumerable<Author>> GetAuthorFollowers(string authorName)
    {
        var author = await GetAuthorByName(authorName);
        return author.Followers.ToList();
    }

    public async Task<IEnumerable<Author>> GetAuthorFollowing(string authorName)
    {
        var author = await GetAuthorByName(authorName);
        return author.Following.ToList();
    }
}
