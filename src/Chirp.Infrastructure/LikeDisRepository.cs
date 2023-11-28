using Enums.ACO;
using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Chirp.ODTO;

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

    // [FIX] Have to make a method that gets the Cheep Info when the user is NOT signed in.
    public async Task<CheepOpinionDTO> GetCheepLikesAndDislikes(int CheepId)
    {
        var cld = await GetCheepLikeDis(CheepId) ?? throw new Exception("File: 'LikeDisRepository' - Method: 'GetNumCheepLikesDis' - Message: CheepLikeDisSchema could not be located with CheepId");
        return new CheepOpinionDTO(cld, AuthorCheepOpinion.NULL, cld.Likes.Count, cld.Dislikes.Count);
    }

    public async Task<CheepOpinionDTO> GetAuthorCheepOpinion(int CheepId, string AuthorName)
    {
        try 
        {
            var author = await _authorRepository.GetAuthorByName(AuthorName) ?? throw new Exception($"Could not find an Author with UserName {AuthorName}");
            var co_Info = await GetCheepLikesAndDislikes(CheepId);

            CheepLikeDis co_Schema       = co_Info.CheepOpinionSchema;
            AuthorCheepOpinion a_Opinion = co_Info.AuthorCheepOpinion;
            int c_NumLikes               = co_Info.NumLikes;
            int c_NumDislikes            = co_Info.NumDislikes;

            if(co_Schema.Likes.Contains(author))
            {
                return new CheepOpinionDTO(co_Schema, AuthorCheepOpinion.LIKES, c_NumLikes, c_NumDislikes);
            } 
            else if(co_Schema.Dislikes.Contains(author))
            {
                return new CheepOpinionDTO(co_Schema, AuthorCheepOpinion.DISLIKES, c_NumLikes, c_NumDislikes);
            }
            else
            {
                return new CheepOpinionDTO(co_Schema, AuthorCheepOpinion.NEITHER, c_NumLikes, c_NumDislikes);
            }
        } 
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}