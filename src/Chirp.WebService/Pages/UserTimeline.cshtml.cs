using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly DatabaseContext _context;

    public Author Author { get; set; }

    public IList<Cheep> Cheeps { get; set; } = null!;

    public UserTimelineModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
     
         Cheeps = _context.Cheeps.Include(cheeps => cheeps.Author).ToList();
         
        return Page();
    }
}
