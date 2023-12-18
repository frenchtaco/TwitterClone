using Microsoft.EntityFrameworkCore;

using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.CDTO;
using Microsoft.Extensions.Logging;
using Enums.ACO;
using Chirp.ODTO;
using System.Formats.Asn1;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly DatabaseContext _context;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeDisRepository _likeDisRepository;

    public CheepRepository(DatabaseContext context, IAuthorRepository authorRepository, ILikeDisRepository likeDisRepository)
    {
        _context = context;
        _authorRepository = authorRepository;
        _likeDisRepository = likeDisRepository;
    }

    public int CheepsPerPage()
    {
        return 32;
    }

    public async Task<IEnumerable<Cheep>> GetCheeps(int page, string orderBy)
    {
        var cheepQuery = _context.Cheeps
            .Include(cheep => cheep.Author)
            .Include(cheep => cheep.LikesAndDislikes) 
                .ThenInclude(cheepLikeDis => cheepLikeDis.Likes) 
            .Include(cheep => cheep.LikesAndDislikes) 
                .ThenInclude(cheepLikeDis => cheepLikeDis.Dislikes);
            
        switch(orderBy)
        {
            case "timestamp":
                return await cheepQuery
                    .OrderByDescending(cheep => cheep.TimeStamp)
                    .Skip(page * CheepsPerPage())
                    .Take(CheepsPerPage())
                    .ToListAsync();
            case "liked":
                return await cheepQuery
                    .OrderByDescending(cheep => cheep.LikesAndDislikes.Likes.Count)
                    .Skip(page * CheepsPerPage())
                    .Take(CheepsPerPage())
                    .ToListAsync();
            case "disliked":
                return await cheepQuery
                    .OrderByDescending(cheep => cheep.LikesAndDislikes.Dislikes.Count)
                    .Skip(page * CheepsPerPage())
                    .Take(CheepsPerPage())
                    .ToListAsync();
            case "name":
                return await cheepQuery
                    .OrderBy(cheep => cheep.Author.UserName)
                    .Skip(page * CheepsPerPage())
                    .Take(CheepsPerPage())
                    .ToListAsync();
            default:
                return await cheepQuery
                    .OrderByDescending(cheep => cheep.TimeStamp)
                    .Skip(page * CheepsPerPage())
                    .Take(CheepsPerPage())
                    .ToListAsync();
        }
    }

    public async Task<IEnumerable<Cheep>> GetAllCheepsFromAuthor(string AuthorUserName)
    {
        return await _context.Cheeps
            .Include(cheep => cheep.Author)
            .Where(cheep => cheep.Author.UserName == AuthorUserName)
            .ToListAsync();
    }

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

    public async Task<Cheep> CreateCheep(CheepDTO cheepDTO)
    {
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

            // 02. Link Author to Cheep:
            author.Cheeps ??= new List<Cheep>();
            author.Cheeps.Add(newCheep);

            // 03. Update Context:
            _context.Cheeps.Add(newCheep);
            _context.CheepLikeDis.Add(_likeDisRepository.CreateLikeDisSchema(newCheep));

            // 04. Save the changes:
            await _context.SaveChangesAsync();

            // 05. Return the newly created Cheep:
            return newCheep;
        }
        catch (Exception ex)
        {
            throw new Exception($"File: CheepRepository.cs - Method: 'CreateCheep()' - Stack Trace: {ex.StackTrace}");
        }

    }

    public async Task<bool> DeleteAllCheepsFromAuthor(string AuthorUserName)
    {
        try
        {
            bool IsSuccess = false;
            var authorToForget = await _authorRepository.GetAuthorByName(AuthorUserName);
            var authorCheeps = await GetAllCheepsFromAuthor(AuthorUserName);

            int cheepCounter = 0;

            foreach (Cheep cheep in authorCheeps)
            {
                cheepCounter++;
                int cID = cheep.CheepId;
                
                CheepLikeDis cheepOpinionSchema = await _likeDisRepository.GetCheepLikeDis(cheep.CheepId) ?? throw new Exception("The 'CheepLikeDisSchema' could not be located");
                
                if(cheepOpinionSchema != null)
                {
                    cheepOpinionSchema.Likes.Clear();
                    cheepOpinionSchema.Dislikes.Clear();
                    
                    _context.CheepLikeDis.Remove(cheepOpinionSchema);

                    IsSuccess = true;
                } 
                else 
                { 
                    IsSuccess = false; 
                    break; 
                }

                var removedCheep = _context.Cheeps.Remove(cheep);
            }

            await _context.SaveChangesAsync();

            return IsSuccess;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteCheep(string AuthorUserName, int CheepId)
    {
        try
        {
            var author = await _authorRepository.GetAuthorByName(AuthorUserName);
            var cheepToBeDeleted = await GetCheepById(CheepId);

            bool IsSuccess = false;

            if(author.Cheeps.Contains(cheepToBeDeleted))
            {
                IsSuccess = author.Cheeps.Remove(cheepToBeDeleted);
                
                if(IsSuccess)
                {
                    var removedCheep = _context.Cheeps.Remove(cheepToBeDeleted);
                    // Add fail-safe
                    await _context.SaveChangesAsync();
                }
            }


            return IsSuccess;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task GiveOpinionOfCheep(bool IsLike, int CheepId, string AuthorName) // [TODO] Combine both LikeCheep and DislikeCheep and just make them pass an additional variable.
    {
        try
        {
            Author author = await _authorRepository.GetAuthorByName(AuthorName);
            CheepOpinionDTO CODTO = await _likeDisRepository.GetAuthorCheepOpinion(CheepId, AuthorName);

            // [!BEWARE!] Potential need for Fail-Safe


            switch(CODTO.AuthorCheepOpinion)
            {

                // Case .01: They Liked it but now they don't:
                case AuthorCheepOpinion.LIKES:  
                    if(!IsLike)
                    {
                        CODTO.CheepOpinionSchema.Likes.Remove(author);
                        CODTO.CheepOpinionSchema.Dislikes.Add(author);
                    } 
                    else 
                    {
                        CODTO.CheepOpinionSchema.Likes.Remove(author);
                    }
                    break;

                // Case .02: They Disliked it but now they do like it:
                case AuthorCheepOpinion.DISLIKES:
                    if(IsLike)
                    {
                        CODTO.CheepOpinionSchema.Dislikes.Remove(author);
                        CODTO.CheepOpinionSchema.Likes.Add(author);
                    } 
                    else 
                    { 
                        CODTO.CheepOpinionSchema.Dislikes.Remove(author);
                    }
                    break;
                    
                // Case .03: They did neither and now they either Like or Dislike:
                case AuthorCheepOpinion.NEITHER:
                    if(IsLike)
                    {
                        CODTO.CheepOpinionSchema.Likes.Add(author);
                    } 
                    else
                    {
                        CODTO.CheepOpinionSchema.Dislikes.Add(author);
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}