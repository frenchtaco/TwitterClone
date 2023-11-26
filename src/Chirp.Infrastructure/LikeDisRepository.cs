using Enums.ACO;
using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public class LikeDisRepository : ILikeDisRepository
{
    private readonly ILogger<LikeDisRepository> _logger;
    private readonly IAuthorRepository _authorRepository;
    private readonly DatabaseContext _context;

    public LikeDisRepository(ILogger<LikeDisRepository> logger, DatabaseContext context, IAuthorRepository authorRepository)
    {
        _logger = logger;
        _context = context;
        _authorRepository = authorRepository;
    }

    public CheepLikeDis CreateLikeDisSchema(Cheep cheep)
    {
        return (
            new CheepLikeDis
            {
                Cheep = cheep,
                CheepId = cheep.CheepId,
                Likes = new HashSet<Author>(),
                Dislikes = new HashSet<Author>()
            }
        );
    }

    

    public async Task<CheepLikeDis?> GetCheepLikeDis(int CheepId)
    {
        return await _context.CheepLikeDis
            .Include(cld => cld.Cheep)
            .Include(cld => cld.Likes)
            .Include(cld => cld.Dislikes)
            .Where(cld => cld.CheepId == CheepId)
            .FirstOrDefaultAsync();
    }

    public async Task<AuthorCheepOpinion> GetAuthorCheepOpinion(int CheepId, string AuthorName)
    {
        try 
        {
            var author = await _authorRepository.GetAuthorByName(AuthorName) ?? throw new Exception($"Could not find an Author with UserName {AuthorName}");
            var cheepLikeDisSchema = await GetCheepLikeDis(CheepId);   

            if(cheepLikeDisSchema != null)
            {
                _logger.LogInformation($"Cheep {CheepId} --> 'Likes': {cheepLikeDisSchema.Likes.Count} ### 'Dislikes': {cheepLikeDisSchema.Dislikes.Count}");
                if(cheepLikeDisSchema.Likes.Contains(author))
                {
                    _logger.LogInformation($"Author {author.UserName} was located in 'Likes'");
                    return AuthorCheepOpinion.LIKES;
                } 
                else if(cheepLikeDisSchema.Dislikes.Contains(author))
                {
                    _logger.LogInformation($"Author {author.UserName} was located in 'Dislikes'");
                    return AuthorCheepOpinion.DISLIKES;
                }
                else
                {
                    return AuthorCheepOpinion.NEITHER;
                }
            } 
            else
            {
                throw new Exception("[ERROR] Variable 'cheepLikeDisSchema' was NULL");
            }
        } 
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}