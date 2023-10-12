using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;
namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly DatabaseContext _context;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int cheepsPerPage;
    public int totalCheeps;
    public UserTimelineModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(String author, [FromQuery] int page)
    {
        Cheeps = _context.Cheeps.Where(cheep => cheep.Author.Name == author).ToList();

        totalCheeps = Cheeps.Count;
        cheepsPerPage = 32;

        if (Cheeps.Count >= page * cheepsPerPage) 
        {
            Cheeps = Cheeps.GetRange((page - 1) * cheepsPerPage, cheepsPerPage);
        }
        else 
        {
            int cheepsLeft = cheepsPerPage - (page * cheepsPerPage - Cheeps.Count);
            Cheeps = Cheeps.GetRange((page - 1) * cheepsPerPage, cheepsLeft);
        }

        return Page();
    }
}
