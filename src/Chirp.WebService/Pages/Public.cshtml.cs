using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly DatabaseContext _context;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int totalCheeps;
    public int cheepsPerPage;

    public PublicModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnGet([FromQuery] int page = 1)
    {
        // Allows access to our Authors and their subsequent Cheeps:
        Cheeps = _context.Cheeps.ToList();

        totalCheeps = Cheeps.Count;
        cheepsPerPage = 32;

        if (page == 0) 
        {
            page = 1;
        }
        
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
