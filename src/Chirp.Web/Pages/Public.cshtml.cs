using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using DBContext;
using Chirp.Models;
using Chirp.Interfaces;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    public List<Cheep> Cheeps { get; set; } = null!;
    public int totalCheeps;
    public int cheepsPerPage;

    public PublicModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
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
}
