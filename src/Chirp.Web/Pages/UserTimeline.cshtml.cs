using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using DBContext;
using Chirp.Models;
using Chirp.Interfaces;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly DatabaseContext _context;
    private readonly ILogger<UserTimelineModel> _logger;

    public List<Cheep> Cheeps { get; set; } = null!;
    private ICheepRepository cheepRepository;
    public int cheepsPerPage;
    public int totalCheeps;
    public UserTimelineModel(DatabaseContext context, SignInManager<Author> signInManager, UserManager<Author> userManager, ILogger<UserTimelineModel> logger)
    {
        // Should "CheepRepository" be changed so it uses DI? 
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
        
        cheepRepository = new CheepRepository(context);
        cheepsPerPage = cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet(string author, [FromQuery] int page)
    {
        if(_signInManager.IsSignedIn(User)) { _logger.LogInformation("User is logged in"); }
        else { _logger.LogInformation("User is NOT logged in"); }

        IEnumerable<Cheep> cheeps = await cheepRepository.GetCheepsFromAuthor(author, page);
        Cheeps = cheeps.ToList();

        IEnumerable<Cheep> allCheeps = await cheepRepository.GetAllCheepsFromAuthor(author);
        totalCheeps = allCheeps.Count();

        return Page();
    }
}
