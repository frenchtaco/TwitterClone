using Microsoft.EntityFrameworkCore;

using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.CDTO;
using Microsoft.Extensions.Logging;
using Enums.ACO;

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
    public async Task<IEnumerable<Cheep>> GetAllCheeps()
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .ToListAsync();
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
    public async Task<IEnumerable<Cheep>> GetAllCheepsFromAuthor(string author)
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .Include(cheep => cheep.CheepId)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .ToListAsync();
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
            CheepLikeDis? cheepOpinionSchema = await _likeDisRepository.GetCheepLikeDis(CheepId);       // [TODO] Add measure to check existance / validity.

            // [TODO] Make this nicer - This is just an additional "fail-safe" incase it goes wrong.
            if(cheepOpinionSchema == null)
            {
                Cheep cheep = await GetCheepById(CheepId);
                cheepOpinionSchema ??= _likeDisRepository.CreateLikeDisSchema(cheep);
                _context.CheepLikeDis.Add(cheepOpinionSchema);
            }

            AuthorCheepOpinion aco = await _likeDisRepository.GetAuthorCheepOpinion(CheepId, AuthorName);

            string testPrint = "";

            switch(aco)
            {

                // Case .01: They Liked it but now they don't:
                case AuthorCheepOpinion.LIKES:  
                    if(!IsLike)
                    {
                        cheepOpinionSchema.Likes.Remove(author);
                        cheepOpinionSchema.Dislikes.Add(author);
                        testPrint += "Author was added to 'Dislikes' and removed from 'Likes'";
                    } else { testPrint += "Author already in 'Likes'"; }
                    _logger.LogInformation($"Author {author.UserName} 'Likes' this Cheep");
                    break;

                // Case .02: They Disliked it but now they do like it:
                case AuthorCheepOpinion.DISLIKES:
                    if(IsLike)
                    {
                        cheepOpinionSchema.Dislikes.Remove(author);
                        cheepOpinionSchema.Likes.Add(author);
                        testPrint += "Author was added to 'Likes' and removed from 'Dislikes'";
                    } else { testPrint += "Author already in 'Dislikes'"; }
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

                    _logger.LogInformation($"Author {author.UserName} had NO opinion of this Cheep");
                    break;
            }

            _logger.LogInformation(testPrint);
            _logger.LogInformation($"[SIZE TEST] Size of Cheep {CheepId}'s 'Like' HashSet: {cheepOpinionSchema.Likes.Count()} and 'Dislikes' HashSet: {cheepOpinionSchema.Dislikes.Count()}");

            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}