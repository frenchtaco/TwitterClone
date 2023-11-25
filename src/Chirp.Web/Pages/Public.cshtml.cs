using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Chirp.Models;
using Chirp.Interfaces;
using Chirp.CDTO;
using Chirp.FDTO;
using Enums.ACO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    // 01. Repositories and Managers:
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeDisRepository _likeDisRepository;
    private readonly UserManager<Author> _userManager;
    private readonly SignInManager<Author> _signInManager;
    private readonly ILogger<PublicModel> _logger;

    // 02. Variables:
    public List<Cheep> Cheeps { get; set; } = null!;
    public Author SignedInAuthor { get; set; } = null!;
    public Dictionary<int, AuthorCheepOpinion>? AuthorOpinionOfCheeps { get; set; }
    public int totalCheeps;
    public int cheepsPerPage;

    // 03. Bind properties:
    [BindProperty, Required(ErrorMessage="Cheep must be between 1-to-160 characters"), StringLength(160, MinimumLength = 1)]
    public string CheepText { get; set; } = "";
    [BindProperty]
    public bool IsFollow { get; set; } = false;
    [BindProperty]
    public string TargetAuthorUserName { get; set; } = null!;

    public PublicModel(
        ILogger<PublicModel> logger, 
        ILikeDisRepository likeDisRepository,
        IAuthorRepository authorRepository, 
        ICheepRepository cheepRepository, 
        UserManager<Author> userManager, 
        SignInManager<Author> signInManager)
    {
        _logger = logger;
        _likeDisRepository = likeDisRepository;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _userManager = userManager;
        _signInManager = signInManager;

        cheepsPerPage = cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet([FromQuery] int? page = 0)
    {   
        int pgNum = page ?? 0;
        
        IEnumerable<Cheep> cheeps = await _cheepRepository.GetCheeps(pgNum);
        Cheeps = cheeps.ToList();

        IEnumerable<Cheep> allCheeps = await _cheepRepository.GetAllCheeps();
        totalCheeps = allCheeps.Count();

        AuthorOpinionOfCheeps = new Dictionary<int, AuthorCheepOpinion>();

        if(_signInManager.IsSignedIn(User))
        {
            try
            {
                SignedInAuthor = await _authorRepository.GetAuthorByName(User.Identity?.Name);

                if(Cheeps.Any())
                {
                    foreach (var cheep in Cheeps)
                    {
                        var opinion = await _likeDisRepository.GetAuthorCheepOpinion(cheep.CheepId, SignedInAuthor.UserName);
                        AuthorOpinionOfCheeps.Add(cheep.CheepId, opinion);
                    }
                }
            }
            catch(Exception ex)
            {
                string exceptionInfo = $"File: Public.cshtml.cs \n\n Method: 'OnGet()' \n\n Message: {ex.Message} \n\n Stack Trace: {ex.StackTrace}";
                TempData["ErrorMessage"] = exceptionInfo;
                return RedirectToPage("/Error");
            }
        }

        // [TEST]
        

        return Page();
    }

    public async Task<IActionResult> OnPostCheep([FromQuery] int? page = 0) 
    {
        ModelState.Clear();
        
        int pgNum = page ?? 0;

        try
        {
            if(ModelState.IsValid) {
                var currUser = await _userManager.GetUserAsync(User) ?? throw new Exception("ERROR: User could not be found");

                CheepDTO cheepDTO = new(CheepText, currUser.UserName);  // [TODO] Change to User.Identity.?.Name;

                await _cheepRepository.CreateCheep(cheepDTO);

                return RedirectToPage("Public", new { pgNum });
            }
            else if(!ModelState.IsValid)
            {
                //... iterate over model states
            }
        }
        catch(Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage("/Error");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostFollow([FromQuery] int? page = 0)
    {
        ModelState.Clear();

        try
        {
            if(ModelState.IsValid) {
                if(_signInManager.IsSignedIn(User))
                {
                    FollowersDTO followersDTO = new(User.Identity.Name, TargetAuthorUserName);  // [TODO] Remove warning but we still want it to be caught by exception.

                    if(IsFollow) 
                    {
                        await _authorRepository.Follow(followersDTO);
                    }
                    else
                    {                        
                        await _authorRepository.Unfollow(followersDTO);
                    }
                } 
                else if(SignedInAuthor == null)
                {
                    throw new Exception("[PUBLIC.CSHTML.CS] The 'SignedInAuthor' variable was NULL");
                }

                return RedirectToPage("Public", new { page });
            } 
        }
        catch(Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage("/Error");
        }

        return RedirectToPage("Public", new { page });
    }

    public async Task<IActionResult> OnPostDislikeOrLike([FromQuery] int? page = 0)
    {
        ModelState.Clear();

        try
        {
            _logger.LogInformation("Like / Dislike method was called");
            if(ModelState.IsValid) {
                if(_signInManager.IsSignedIn(User))
                {   
                    _logger.LogInformation($"Target Author UserName: {TargetAuthorUserName}");

                    var likeDislikeValue = Request.Form["likeDis"];
                    if(string.IsNullOrEmpty(likeDislikeValue)) _logger.LogInformation("String was NULL / Empty");

                    if(likeDislikeValue == "like")
                    {
                        
                        _logger.LogInformation("Like was clicked");
                    }
                    else if(likeDislikeValue == "dislike")
                    {
                        _logger.LogInformation("Dislike was clicked");
                    }

                    var dev = await _cheepRepository.GetCheepsFromAuthor(User.Identity?.Name, (int) page);
                } 

                return RedirectToPage("Public", new { page });
            } 
            else if(!ModelState.IsValid)
            {
                throw new Exception("ModelState was invalid");
            }
        }
        catch(Exception ex)
        {
            string exceptionInfo = "File: Public.cshtml.cs \n Method: 'OnPostDislikeOrLike()' \n Stack Trace: \n";
            TempData["ErrorMessage"] = exceptionInfo += ex.StackTrace;
            return RedirectToPage("/Error");
        }

        return RedirectToPage("Public", new { page });
    }
}
