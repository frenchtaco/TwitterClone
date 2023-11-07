using Chirp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class MyPageModel : PageModel
{
    private readonly ILogger<MyPageModel> _logger;
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    

    public MyPageModel(ILogger<MyPageModel> logger, SignInManager<Author> signInManager, UserManager<Author> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public void OnGet()
    {
        if(_signInManager.IsSignedIn(User)) { _logger.LogInformation("[MYPAGE] User is logged in"); }
        else { _logger.LogInformation("[MYPAGE] User is NOT logged in"); }
    }
}

