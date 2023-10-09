using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext;
using Chirpin.Models;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly DatabaseContext _context;
    public Cheep Cheeps { get; set; }
    public Author Authors { get; set; }

    public UserTimelineModel(DatabaseContext context)
    {
        _context = context;
    }

    public IActionResult OnPost()
    {
        return Page();
    }
}
