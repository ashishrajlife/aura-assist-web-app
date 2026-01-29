using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aura_assist_prod.Models
{
    public class InfluencerProfile
    {
        [Key]
        public int InfluencerProfileId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [MaxLength(100)]
        public string InstagramHandle { get; set; }

        [MaxLength(100)]
        public string TwitterHandle { get; set; }

        [MaxLength(100)]
        public string WhatsappHandle { get; set; }

        public int FollowersCount { get; set; } = 0;

        [MaxLength(50)]
        public string Category { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; } = 0;
    }
}
