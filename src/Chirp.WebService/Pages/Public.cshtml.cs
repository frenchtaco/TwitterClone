using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirp.Models;
using Microsoft.EntityFrameworkCore;

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

    public IActionResult OnGet([FromQuery] int page)
    {
        Cheeps = _context.Cheeps.Include(cheep => cheep.Author).ToList();

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
