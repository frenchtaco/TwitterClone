using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirpin.Models 
{
    [Table("Cheeps")]
    public class Cheep
    {
        [Key]
        public int CheepId { get; set; }
        public required Author Author { get; set; }
        public required string Text { get; set; }
        [DataType(DataType.Date)]
        public required DateTime TimeStamp { get; set; }
    }
}