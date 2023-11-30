using Microsoft.EntityFrameworkCore;

using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.CDTO;
using Microsoft.Extensions.Logging;
using Enums.ACO;
using Chirp.ODTO;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ILogger<CheepRepository> _logger;
    private readonly DatabaseContext _context;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeDisRepository _likeDisRepository;

    public CheepRepository(DatabaseContext context, IAuthorRepository authorRepository, ILikeDisRepository likeDisRepository, ILogger<CheepRepository> logger)
    {
        _logger = logger;
        _context = context;
        _authorRepository = authorRepository;
        _likeDisRepository = likeDisRepository;
    }

    public int CheepsPerPage()
    {
        return 32;
    }

    public async Task<IEnumerable<Cheep>> GetCheeps(int page)
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip(page * CheepsPerPage())
            .Take(CheepsPerPage())
            .ToListAsync();
    }

    // [TODO] Change to return an integer as its only used as a way to calculate pagination.
    public async Task<int> GetTotalNumberOfCheeps()
    {
        return await _context.Cheeps
            .CountAsync();
    }

    public async Task<IEnumerable<Cheep>> GetCheepsFromAuthor(string author, int page)
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip(page * CheepsPerPage())
            .Take(CheepsPerPage())
            .ToListAsync();
    }

    // [TODO] Same as above, make it return an integer as its only used to calculate pagination.
    public async Task<int> GetTotalNumberOfAuthorCheeps(string author)
    {
        return await _context.Cheeps
            .Where(cheep => cheep.Author.UserName == author)
            .CountAsync();
    }

    public async Task<IEnumerable<Cheep>> GetTop4FromAuthor(string author)
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Take(4)
            .ToListAsync();
    }

    public async Task<Cheep> GetCheepById(int CheepId)
    {
        return await _context.Cheeps
            .Where(c => c.CheepId == CheepId)
            .FirstOrDefaultAsync() ?? throw new Exception("File: 'CheepRepository.cs' - Method: 'GetCheepById' - Message: 'Cheep couldn't be located with CheepId and was NULL'");
    }

    public async Task CreateCheep(CheepDTO cheepDTO)
    {
        _logger.LogInformation($"[BEFORE] Num. Cheeps: {_context.Cheeps.Count()} - Num. CheepOpinionSchemas{_context.CheepLikeDis.Count()}");
        try
        {
            var author = await _authorRepository.GetAuthorByName(cheepDTO.Author) ?? throw new Exception("Author was NULL");

            // 01. Create new Cheep:
            Cheep newCheep = new()
            {
                Author = author,
                Text = cheepDTO.Text,
                TimeStamp = DateTime.UtcNow,
            };

            //02. Link Author to Cheep:
            author.Cheeps ??= new List<Cheep>();
            author.Cheeps.Add(newCheep);

            //03. Update Context:
            _context.Cheeps.Add(newCheep);
            _context.CheepLikeDis.Add(_likeDisRepository.CreateLikeDisSchema(newCheep));

            //04. Save the changes:
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"File: CheepRepository.cs - Method: 'CreateCheep()' - Stack Trace: {ex.StackTrace}"
            );
        }
        _logger.LogInformation($"[AFTER] Num. Cheeps: {_context.Cheeps.Count()} - Num. CheepOpinionSchemas{_context.CheepLikeDis.Count()}");
    }

    public async Task GiveOpinionOfCheep(bool IsLike, int CheepId, string AuthorName) // [TODO] Combine both LikeCheep and DislikeCheep and just make them pass an additional variable.
    {
        try
        {
            Author author = await _authorRepository.GetAuthorByName(AuthorName);
            CheepOpinionDTO CODTO = await _likeDisRepository.GetAuthorCheepOpinion(CheepId, AuthorName);

            // [!BEWARE!] Potential need for Fail-Safe

            string testPrint = "";

            switch(CODTO.AuthorCheepOpinion)
            {

                // Case .01: They Liked it but now they don't:
                case AuthorCheepOpinion.LIKES:  
                    if(!IsLike)
                    {
                        CODTO.CheepOpinionSchema.Likes.Remove(author);
                        CODTO.CheepOpinionSchema.Dislikes.Add(author);
                        testPrint += "Author was added to 'Dislikes' and removed from 'Likes'";
                    } 
                    else 
                    {
                        CODTO.CheepOpinionSchema.Likes.Remove(author);
                        testPrint += "Author already in 'Likes', so removed them from this Cheeps 'Likes'"; 
                    }
                    break;

                // Case .02: They Disliked it but now they do like it:
                case AuthorCheepOpinion.DISLIKES:
                    if(IsLike)
                    {
                        CODTO.CheepOpinionSchema.Dislikes.Remove(author);
                        CODTO.CheepOpinionSchema.Likes.Add(author);
                        testPrint += "Author was added to 'Likes' and removed from 'Dislikes'";
                    } 
                    else 
                    { 
                        CODTO.CheepOpinionSchema.Dislikes.Remove(author);
                        testPrint += "Author already in 'Dislikes', so removed them from this Cheeps 'Dislikes'"; 
                    }
                    _logger.LogInformation($"Author {author.UserName} 'Dislikes' this Cheep");
                    break;
                    
                // Case .03: They did neither and now they either Like or Dislike:
                case AuthorCheepOpinion.NEITHER:
                    if(IsLike)
                    {
                        CODTO.CheepOpinionSchema.Likes.Add(author);
                        testPrint += "Author was added to 'Likes'";
                    } 
                    else
                    {
                        CODTO.CheepOpinionSchema.Dislikes.Add(author);
                        testPrint += "Author was added to 'Dislikes'";
                    }

                    _logger.LogInformation($"Author {author.UserName} had NO opinion of this Cheep");
                    break;
            }

            _logger.LogInformation(testPrint);
            _logger.LogInformation($"[SIZE TEST] Size of Cheep {CheepId}'s 'Like' HashSet: {CODTO.CheepOpinionSchema.Likes.Count()} and 'Dislikes' HashSet: {CODTO.CheepOpinionSchema.Dislikes.Count()}");

            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}