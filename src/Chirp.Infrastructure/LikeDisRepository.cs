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

    public async Task GiveOpinionOfCheep(bool IsLike, int CheepId, string AuthorName) // [TODO] Combine both LikeCheep and DislikeCheep and just make them pass an additional variable.
    {
        Author author = await _authorRepository.GetAuthorByName(AuthorName);
        CheepLikeDis cheepOpinionSchema = await GetCheepLikeDis(CheepId);       // [TODO] Add measure to check existance / validity.

        AuthorCheepOpinion aco = await GetAuthorCheepOpinion(CheepId, AuthorName);

        string testPrint = "";

        switch(aco)
        {
            // Case .01: They Liked it but now they don't:
            case AuthorCheepOpinion.LIKES:  
                if(!IsLike)
                {
                    cheepOpinionSchema.Likes.Remove(author);
                    cheepOpinionSchema.Dislikes.Add(author);
                } 
                _logger.LogInformation($"Author {author.UserName} 'Likes' this Cheep");
                break;
            // Case .02: They Disliked it but now they do like it:
            case AuthorCheepOpinion.DISLIKES:
                if(IsLike)
                {
                    cheepOpinionSchema.Dislikes.Remove(author);
                    cheepOpinionSchema.Likes.Add(author);
                } 
                _logger.LogInformation($"Author {author.UserName} 'Dislikes' this Cheep");
                break;
            // Case .03: They did neither and now they either Like or Dislike:
            case AuthorCheepOpinion.NEITHER:
                if(IsLike)
                {
                    cheepOpinionSchema.Likes.Add(author);
                    testPrint += "Author was added to 'Likes'";
                } 
                else 
                {
                    cheepOpinionSchema.Dislikes.Add(author);
                    testPrint += "Author was added to 'Dislikes'";
                }

                _logger.LogInformation($"Author {author.UserName} has NO opinion of this Cheep");
                break;
        }

        await _context.SaveChangesAsync();
    }


    public async Task<CheepLikeDis?> GetCheepLikeDis(int CheepId)
    {
        return await _context.CheepLikeDis
            .Include(cld => cld.Cheep)
            .Where(cld => cld.CheepId == CheepId)
            .FirstOrDefaultAsync();
    }

    public async Task<AuthorCheepOpinion> GetAuthorCheepOpinion(int CheepId, string AuthorName)
    {
        try 
        {
            var author = await _authorRepository.GetAuthorByName(AuthorName) ?? throw new Exception($"File:' LikeDisRepository.cs' - Method: 'GetAuthorCheepOpinion()' - Message: Could not find an Author with UserName {AuthorName}");
            var cheepLikeDisSchema = await GetCheepLikeDis(CheepId);   

            if(cheepLikeDisSchema != null)
            {
                // [TODO] Test without this:
                cheepLikeDisSchema.Likes ??= new HashSet<Author>();
                cheepLikeDisSchema.Dislikes ??= new HashSet<Author>();

                if(cheepLikeDisSchema.Likes.Any() || cheepLikeDisSchema.Likes.Contains(author))
                {
                    return AuthorCheepOpinion.LIKES;
                } 
                else if(cheepLikeDisSchema.Dislikes.Any() || cheepLikeDisSchema.Dislikes.Contains(author))
                {
                    return AuthorCheepOpinion.DISLIKES;
                }
                else
                {
                    return AuthorCheepOpinion.NEITHER;
                }
            } 
            else
            {
                _logger.LogInformation("[ERROR] Variable 'cheepLikeDisSchema' was NULL");
                throw new Exception("Variable 'cheepLikeDisSchema' was NULL");
            }
        } 
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}