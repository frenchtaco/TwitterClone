using Chirp.Models;
using Microsoft.AspNetCore.Identity;

namespace FUV;
public class ForgottenUserValidator<TUser> : IUserValidator<TUser> where TUser : class
{
    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var author = user as Author;

        Console.WriteLine("Validation was called!");

        if(author != null && author.IsForgotten)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "ForgottenUser",
                Description = "This user has been forgotten."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}