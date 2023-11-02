using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Models 
{
    [Table("Authors")]
    public class Author : IdentityUser
    {
        [Display(Name = "Author ID")]
        public int AuthorId { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public override required string Email { get; set; } = null!;
        public required ICollection<Cheep> Cheeps { get; set;}
    }
}


