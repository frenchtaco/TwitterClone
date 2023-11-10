using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Models
{
    [Table("Authors")]
    public class Author : IdentityUser
    {
        [Display(Name = "Author ID")]
        public int AuthorId { get; set; }   // [TODO]: Make this 'override' the attribute from 'IdentityUser'

        [StringLength(20, MinimumLength = 5)]
        [Display(Name = "Username")]
        public required override string UserName { get; set; }

        [EmailAddress]
        public required override string Email { get; set; }
        public ICollection<Cheep> Cheeps { get; set; } = null!;
    }
}