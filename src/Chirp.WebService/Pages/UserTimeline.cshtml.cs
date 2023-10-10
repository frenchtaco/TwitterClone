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
    public int lastPage;

    public UserTimelineModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(String author, [FromQuery] int page)
    {
        Cheeps = _context.Cheeps.Where(cheep => cheep.Author.Name == author).ToList();

        if (Cheeps.Count >= page * 32) 
        {
            Cheeps = Cheeps.GetRange((page - 1) * 32, 32);
        }
        else 
        {
            int cheepsLeft = 32 - (page * 32 - Cheeps.Count);
            Cheeps = Cheeps.GetRange((page - 1) * 32, cheepsLeft);
        }

        return Page();
    }
}
