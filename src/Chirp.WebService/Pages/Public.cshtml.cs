using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly DatabaseContext _context;
    public IList<Author> Authors { get; set; } = null!;
    public IList<Cheep> Cheeps { get; set; } = null!;


    public PublicModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        // Allows access to our Authors and their subsequent Cheeps:
        Authors = _context.Authors.Include(author => author.Cheeps).ToList();

        Cheeps = _context.Cheeps.Include(cheeps => cheeps.Author).ToList();

        return Page();
    }
}
