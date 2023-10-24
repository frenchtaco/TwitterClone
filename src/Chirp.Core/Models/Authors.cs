using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirpin.Models 
{
    [Table("Authors")]
    public class Author
    {
        [Display(Name = "Author ID")]
        [Key]
        public int AuthorId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required ICollection<Cheep> Cheeps { get; set;}
    }
}


