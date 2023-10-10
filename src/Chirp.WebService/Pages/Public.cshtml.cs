using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly DatabaseContext _context;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int cheepsPerPage;
    public int totalCheeps;
    public int lastPage;

    public PublicModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet([FromQuery] int page)
    {
        // Allows access to our Authors and their subsequent Cheeps:
        Cheeps = _context.Cheeps.ToList();
        
        if (Cheeps.Count >= page * 32) 
        {
            Cheeps = Cheeps.GetRange((page - 1) * 32, 32);
        }
        else 
        {
            int cheepsLeft = 32 - (page * 32 - Cheeps.Count);
            Cheeps = Cheeps.GetRange((page - 1) * 32, cheepsLeft);
        }

        int totalCheeps = Cheeps.Count;

        return Page();
    }
}
