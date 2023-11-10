using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Models 
{
    [Table("Cheeps")]
    public class Cheep
    {
        [Display(Name = "Cheep ID")]
        [Key]
        public int CheepId { get; set; }
        public required Author Author { get; set; }
        
        [StringLength(160, MinimumLength = 1)]
        public required string Text { get; set; }
        [DataType(DataType.Date)]
        public required DateTime TimeStamp { get; set; }
    }
}