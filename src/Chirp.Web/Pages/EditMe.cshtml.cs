using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class EditMeModel : PageModel
    {

        private readonly ILogger<EditMeModel> _logger;

        [BindProperty]
        public string TargetAuthorUserName { get; set; } = null!;

        [BindProperty]
        public string NewUsername { get; set; }

        //CONSTRUCTOR
        public EditMeModel(ILogger<EditMeModel> logger)
        {
            _logger = logger;
        }


        public async Task<IActionResult> OnPostEditMe([FromQuery] int page = 0)
    {
            _logger.LogInformation($"LOGGED IN USER:  {User.Identity.Name}");
            _logger.LogInformation($"New Username:  {NewUsername}");

            return RedirectToPage("/Public");
        }

        //Inject the correct repositories so that we can fuck around.
       
    }


}
