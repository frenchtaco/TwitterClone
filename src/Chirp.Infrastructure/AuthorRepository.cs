using Chirp.Interfaces;
using Chirp.Models;
using DBContext;
using Chirp.ADTO;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly DatabaseContext _context;

    public AuthorRepository(DatabaseContext context)
    {
        _context = context;
    }

    public void CreateNewAuthor(AuthorDTO authorDTO)
    {
        Author author = new()
        {
            UserName = authorDTO.UserName,
            Email = authorDTO.Email,
            Cheeps = new List<Cheep>(),
            EmailConfirmed = true,
        };

        _context.Authors.Add(author);
        _context.SaveChanges();
    }

    public async Task<Author> GetAuthorByName(string authorName)
    {
        var author = await _context.Authors
        .Where(a => a.UserName == authorName)
        .FirstOrDefaultAsync() ?? null;

        return author;
    }

}
