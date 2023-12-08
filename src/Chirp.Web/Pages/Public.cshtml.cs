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
using Chirp.ODTO;

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
    public Dictionary<int, CheepOpinionDTO> CheepOpinionsInfo { get; set; }
    public int TotalCheeps, CheepsPerPage;
    public bool IsUserSignedIn;

    // 03. Bind properties:
    [BindProperty, Required(ErrorMessage="Cheep must be between 1-to-160 characters"), StringLength(160, MinimumLength = 1)]
    public string CheepText { get; set; } = "";
    [BindProperty]
    [ViewData]
    public bool IsFollow { get; set; } = false;
    [BindProperty]
    public string TargetAuthorUserName { get; set; } = null!;
    [BindProperty]
    public int TargetCheepId { get; set; }

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

        CheepsPerPage = cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet([FromQuery] int? page = 0)
    {
        int pgNum = page ?? 0;
        
        IEnumerable<Cheep> cheeps = await _cheepRepository.GetCheeps(pgNum);
        Cheeps = cheeps.ToList();

        TotalCheeps = await _cheepRepository.GetTotalNumberOfCheeps();
        CheepOpinionsInfo = new Dictionary<int, CheepOpinionDTO>();

        IsUserSignedIn = _signInManager.IsSignedIn(User);

        try
        {
            // 01. We perform Lazy Loading, wherein we retrieve both the Author Opinions on the Cheep and
            //     the total number of 'Likes' and 'Dislikes'.
            if(IsUserSignedIn)
            {
                SignedInAuthor = await _authorRepository.GetAuthorByName(User.Identity?.Name);

                bool Result_PopulateCheepOpinionInfo = await PopulateCheepOpinionInfo(true);
            }
            // 02. Here we just retrieve the 'Likes' and 'Dislikes' associated with that CheepId.
            else if(!IsUserSignedIn)
            {
                bool Result_PopulateCheepOpinionInfo = await PopulateCheepOpinionInfo(false);
            }
        } 
        catch(Exception ex)
        {
            string exceptionInfo = $"File: Public.cshtml.cs \n\n Method: 'OnGet()' \n\n Message: {ex.Message} \n\n Stack Trace: {ex.StackTrace}";
            TempData["ErrorMessage"] = exceptionInfo;
            return RedirectToPage("/Error");
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostCheep([FromQuery] int? page = 0) 
    {
        ModelState.Clear();
        
        try
        {
            if(ModelState.IsValid) {
                var currUser = await _userManager.GetUserAsync(User) ?? throw new Exception("ERROR: User could not be found");

                CheepDTO cheepDTO = new(CheepText, currUser.UserName);  // [TODO] Change to User.Identity.?.Name;

                await _cheepRepository.CreateCheep(cheepDTO);

                return RedirectToPage("Public", new { page });
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
                    FollowersDTO followersDTO = new(User.Identity.Name, TargetAuthorUserName);

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
            if(ModelState.IsValid) {
                if(_signInManager.IsSignedIn(User))
                {   
                    var likeDislikeValue = Request.Form["likeDis"];
                    if(string.IsNullOrEmpty(likeDislikeValue)) throw new Exception("File: 'Public.cshtml.cs' - Method: 'OnPostDislikeOrLike()' - Message: Value retrieved from Request Form was NULL");

                    if(likeDislikeValue == "like")
                    {
                        await _cheepRepository.GiveOpinionOfCheep(true, TargetCheepId, TargetAuthorUserName);
                    }
                    else if(likeDislikeValue == "dislike")
                    {
                        await _cheepRepository.GiveOpinionOfCheep(false, TargetCheepId, TargetAuthorUserName);
                    }
                } 

                return RedirectToPage("Public", new { page });
            } 
            else if(!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "File: 'Public.cshtml.cs' - Method: 'OnPostDislikeOrLike()' - Message: ModelState was Invalid";
                return RedirectToPage("/Error");
            }
        }
        catch(Exception ex)
        {
            string exceptionInfo = "File: Public.cshtml.cs - Method: 'OnPostDislikeOrLike()' - Stack Trace: ";
            TempData["ErrorMessage"] = exceptionInfo += ex.StackTrace;
            return RedirectToPage("/Error");
        }

        return RedirectToPage("Public", new { page });
    }

    public async Task<bool> PopulateCheepOpinionInfo(bool GetAuthorOpinion)
    {
        try
        {
            if(GetAuthorOpinion)
            {
                if(Cheeps.Any())
                {
                    foreach (Cheep cheep in Cheeps)
                    {
                        CheepOpinionDTO co_Info = await _likeDisRepository.GetAuthorCheepOpinion(cheep.CheepId, SignedInAuthor.UserName);
                        if(co_Info == null) throw new Exception("'co_Info' was NULL");
                        CheepOpinionsInfo.Add(cheep.CheepId, co_Info);
                    }
                }
            }
            else
            {
                if(Cheeps.Any())
                {
                    foreach(Cheep cheep in Cheeps)
                    {
                        CheepOpinionDTO LikesAndDislikes = await _likeDisRepository.GetCheepLikesAndDislikes(cheep.CheepId);
                        if(LikesAndDislikes == null) throw new Exception("'LikesAndDislikes' was NULL");
                        CheepOpinionsInfo.Add(cheep.CheepId, LikesAndDislikes);
                    }
                }
            }

            return true;    // [TODO] Make this more fail-safe.
        } 
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PartialViewResult> OnGetOrderCheepsBy([FromQuery] string? orderBy = "")
    {
        try
        {
            SignedInAuthor = await _authorRepository.GetAuthorByName(User.Identity?.Name);
            IsUserSignedIn = _signInManager.IsSignedIn(User);

            await PopulateCheepOpinionInfo(IsUserSignedIn);

            switch(orderBy)
            {
                case "timestamp":
                    _logger.LogInformation("Ordering Cheeps by TimeStamp");
                    Cheeps = Cheeps.OrderBy(c => c.TimeStamp).ToList();
                    break;
                case "likes":
                    _logger.LogInformation("Ordering Cheeps by Likes");
                    Cheeps = Cheeps.OrderBy(c => c.LikesAndDislikes.Likes.Count).ToList();
                    break;
                case "hated":
                    _logger.LogInformation("Ordering Cheeps by Hates");
                    Cheeps = Cheeps.OrderBy(c => c.LikesAndDislikes.Likes.Count).ToList();
                    break;
                case "name":
                    _logger.LogInformation("Ordering Cheeps by UserName");
                    Cheeps = Cheeps.OrderBy(c => c.Author.UserName).ToList();
                    break;
            }
        } 
        catch(Exception ex)
        {
            string exceptionInfo = $"File: Public.cshtml.cs \n\n Method: 'OnGet()' \n\n Message: {ex.Message} \n\n Stack Trace: {ex.StackTrace}";
            TempData["ErrorMessage"] = exceptionInfo;
        }

        ViewData["IsFollow"]             = IsFollow;
        ViewData["TargetAuthorUserName"] = TargetAuthorUserName;
        ViewData["TargetCheepId"]        = TargetCheepId;
        ViewData["CheepOpinionInfo"]     = CheepOpinionsInfo;
        ViewData["Cheeps"]               = Cheeps;
        ViewData["SignedInAuthor"]       = SignedInAuthor;
        ViewData["UserName"]             = User.Identity?.Name;

        return new PartialViewResult {
            ViewName = "_CheepPartial",
        };
    }
}