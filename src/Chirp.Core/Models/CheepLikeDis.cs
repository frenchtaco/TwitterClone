using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Models
{
    [Table("Cheep - Likes & Dislikes")]
    public class CheepLikeDis
    {
        [DisplayName("Cheep - Like & Dislike Id")]
        [Key]
        public int CheepLikeDisId { get; set; }

        [Required]
        public Cheep Cheep { get; set; }
        public int CheepId { get; set; }

        [Required]
        public ISet<Author> Likes { get; set; } = null!;

        [Required]
        public ISet<Author> Dislikes { get; set; } = null!;
    }
}
