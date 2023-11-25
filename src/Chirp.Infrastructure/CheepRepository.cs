using Microsoft.EntityFrameworkCore;

using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.CDTO;
using Microsoft.Extensions.Logging;

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

    public async Task<Cheep?> GetCheepById(int CheepId)
    {
        return await _context.Cheeps
            .Where(c => c.CheepId == CheepId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateCheep(CheepDTO cheepDTO)
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

            //02. Link Author to Cheep:
            author.Cheeps ??= new List<Cheep>();
            author.Cheeps.Add(newCheep);

            //03. Update Context:
            _context.Cheeps.Add(newCheep);
            _context.CheepLikeDis.Add(_likeDisRepository.CreateLikeDisSchema(newCheep));

            // Finalize Changes:
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"File: CheepRepository.cs - Method: 'CreateCheep()' - Stack Trace: {ex.StackTrace}"
            );
        }
    }
}