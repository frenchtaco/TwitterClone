using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Chirp.Models;
using Chirp.Interfaces;
using Chirp.CDTO;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ILogger<PublicModel> _logger;
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly UserManager<Author> _userManager;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int totalCheeps;
    public int cheepsPerPage;

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, UserManager<Author> userManager, ILogger<PublicModel> logger)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _userManager = userManager;
        _logger = logger;

        cheepsPerPage = cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet([FromQuery] int page)
    {        
        IEnumerable<Cheep> cheeps = await _cheepRepository.GetCheeps(page);
        Cheeps = cheeps.ToList();

        IEnumerable<Cheep> allCheeps = await _cheepRepository.GetAllCheeps();
        totalCheeps = allCheeps.Count();

        return Page();
    }


    [BindProperty]
    public string CheepText { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public async Task<IActionResult> OnPostAsync([FromQuery] int? page) 
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var currUser = await _userManager.GetUserAsync(User) ?? throw new Exception("ERROR: User could not be found");

        CheepDTO cheepDTO = new(CheepText, currUser.UserName);

        _cheepRepository.CreateCheep(cheepDTO);

        int pageNumber = page ?? 1;

        return RedirectToPage("Public", new { page = pageNumber });
    }
}
