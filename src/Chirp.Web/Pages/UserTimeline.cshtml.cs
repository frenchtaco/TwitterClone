using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using DBContext;
using Chirp.FDTO;
using Chirp.Models;
using Chirp.Interfaces;
using Chirp.ODTO;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    // 01. Services & Repositories:
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeDisRepository _likeDisRepository;
    private readonly UserManager<Author> _userManager;
    private readonly SignInManager<Author> _signInManager;
    private readonly ILogger<UserTimelineModel> _logger;

    // .02 Variables:
    public Dictionary<int, CO_AuthorOpinion_DTO> AuthorOpinionOfCheeps = null!;
    public Dictionary<int, CO_Schema_DTO> CheepLikesAndDislikes = null!;
    public Dictionary<Author, List<Cheep>> FollowersAndTheirCheeps { get; set; } = null!;
    public List<Cheep> Cheeps { get; set; } = null!;
    public List<Author> Followers { get; set; } = null!;
    public List<Author> Following { get; set; } = null!;
    public Author SignedInUser { get; set; } = null!;
    public Author TimelineUser { get; set; } = null!;
    public int cheepsPerPage;
    public int totalCheeps;
    public UserTimelineModel(UserManager<Author> userManager, SignInManager<Author> signInManager, IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeDisRepository likeDisRepository, ILogger<UserTimelineModel> logger)
    {
        _logger = logger;

        _userManager = userManager;
        _signInManager = signInManager;   
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeDisRepository = likeDisRepository;

        cheepsPerPage = _cheepRepository.CheepsPerPage();
    }

    public async Task<IActionResult> OnGet(string author, [FromQuery] int page)
    {
        try
        {
            // 00. Variables:
            string? signedInUser = User.Identity?.Name;

            // 01. Cheeps:
            IEnumerable<Cheep> cheeps = await _cheepRepository.GetCheepsFromAuthor(author, page);
            Cheeps = cheeps.ToList();

            // 02. All the Authors Cheeps:
            totalCheeps = await _cheepRepository.GetTotalNumberOfAuthorCheeps(author);

            // 03. Followers:
            IEnumerable<Author> followers = await _authorRepository.GetAuthorFollowers(author);
            Followers = followers.ToList();

            // 04. Following:
            IEnumerable<Author> following = await _authorRepository.GetAuthorFollowing(author);
            Following = following.ToList();

            // 05. Signed In User:
            if(_signInManager.IsSignedIn(User) && signedInUser != null && author != signedInUser)
            {
                AuthorOpinionOfCheeps = new Dictionary<int, CO_AuthorOpinion_DTO>();

                TimelineUser = await _authorRepository.GetAuthorByName(author);
                SignedInUser = await _authorRepository.GetAuthorByName(signedInUser);

                if(Followers.Any())
                {
                    foreach (Author follower in Followers)
                    {
                        var followerCheeps = await GetTop4CheepsFromFollower(follower.UserName);
                        FollowersAndTheirCheeps.Add(follower, followerCheeps.ToList());

                        foreach (Cheep followerCheep in followerCheeps)
                        {
                            CO_AuthorOpinion_DTO co_Info = await _likeDisRepository.GetAuthorCheepOpinion(followerCheep.CheepId, follower.UserName);
                            AuthorOpinionOfCheeps.Add(followerCheep.CheepId, co_Info);
                        }
                    }
                }
            } 
            else
            {
                CheepLikesAndDislikes = new Dictionary<int, CO_Schema_DTO>();

                if(Followers.Any())
                {
                    foreach (Author follower in Followers)
                    {
                        var followerCheeps = await GetTop4CheepsFromFollower(follower.UserName);
                        FollowersAndTheirCheeps.Add(follower, followerCheeps.ToList());

                        foreach (Cheep followerCheep in followerCheeps)
                        {
                            CO_Schema_DTO co_CheepLikesAndDislikes = await _likeDisRepository.GetCheepLikesAndDislikes(followerCheep.CheepId);
                            CheepLikesAndDislikes.Add(followerCheep.CheepId, co_CheepLikesAndDislikes);
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            string errorMessage = $"File: 'UserTimeline.cshtml.cs' - Method: 'OnGet' - Message: {ex.Message} - Stack Trace: {ex.StackTrace}";
            TempData["ErrorMessage"] = errorMessage;
            return RedirectToPage("/Error");
        }

        return Page();
    }

    [BindProperty]
    public bool IsFollow { get; set; }
    [BindProperty]
    public string TargetAuthorUserName { get; set; } = null!;
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
                else if(SignedInUser == null)
                {
                    throw new Exception("[USER-TIME-LINE.CSHTML.CS] The 'SignedInUser' variable was NULL");
                }

                return RedirectToPage("UserTimeline", new { page });
            } 
        }
        catch(Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage("/Error");
        }

        return RedirectToPage("UserTimeline", new { page });
    }

    // [TODO] Change this and clean up. Simplist solution - but ugly af.
    public async Task<IEnumerable<Cheep>> GetTop4CheepsFromFollower(string author)
    {
        return await _cheepRepository.GetTop4FromAuthor(author);
    }
}
