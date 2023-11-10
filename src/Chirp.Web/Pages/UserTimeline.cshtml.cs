using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using DBContext;
using Chirp.Models;
using Chirp.Interfaces;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly ILogger<UserTimelineModel> _logger;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int cheepsPerPage;
    public int totalCheeps;
    public UserTimelineModel(ICheepRepository cheepRepository, ILogger<UserTimelineModel> logger)
    {
        // Should "CheepRepository" be changed so it uses DI? 
        // If you click on your name - redirect to MyPage
        // else you go that Users timeline.
        _logger = logger;
        
        _cheepRepository = cheepRepository;
        cheepsPerPage = _cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet(string author, [FromQuery] int page)
    {
        IEnumerable<Cheep> cheeps = await _cheepRepository.GetCheepsFromAuthor(author, page);
        Cheeps = cheeps.ToList();

        IEnumerable<Cheep> allCheeps = await _cheepRepository.GetAllCheepsFromAuthor(author);
        totalCheeps = allCheeps.Count();

        return Page();
    }
}
