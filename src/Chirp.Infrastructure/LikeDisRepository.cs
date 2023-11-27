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
    public async Task<CO_Schema_DTO> GetCheepLikesAndDislikes(int CheepId)
    {
        var cld = await GetCheepLikeDis(CheepId) ?? throw new Exception("File: 'LikeDisRepository' - Method: 'GetNumCheepLikesDis' - Message: CheepLikeDisSchema could not be located with CheepId");
        return new CO_Schema_DTO(cld, cld.Likes.Count, cld.Dislikes.Count);
    }

    public async Task<CO_AuthorOpinion_DTO> GetAuthorCheepOpinion(int CheepId, string AuthorName)
    {
        try 
        {
            var author = await _authorRepository.GetAuthorByName(AuthorName) ?? throw new Exception($"Could not find an Author with UserName {AuthorName}");
            var co_Info = await GetCheepLikesAndDislikes(CheepId);

            CheepLikeDis co_Schema = co_Info.CheepOpinionSchema;
            int c_NumLikes         = co_Info.NumLikes;
            int c_NumDislikes      = co_Info.NumDislikes;

            if(co_Schema != null)
            {
                _logger.LogInformation($"Cheep {CheepId} --> 'Likes': {co_Schema.Likes.Count} ### 'Dislikes': {co_Schema.Dislikes.Count}");
                if(co_Schema.Likes.Contains(author))
                {
                    return new CO_AuthorOpinion_DTO(AuthorCheepOpinion.LIKES, c_NumLikes, c_NumDislikes);
                } 
                else if(co_Schema.Dislikes.Contains(author))
                {
                    return new CO_AuthorOpinion_DTO(AuthorCheepOpinion.DISLIKES, c_NumLikes, c_NumDislikes);
                }
                else
                {
                    return new CO_AuthorOpinion_DTO(AuthorCheepOpinion.NEITHER, c_NumLikes, c_NumDislikes);
                }
            } 
            else
            {
                throw new Exception("[ERROR] Variable 'co_Schema' was NULL");
            }
        } 
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}