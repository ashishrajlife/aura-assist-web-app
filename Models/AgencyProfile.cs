using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aura_assist_prod.Models
{
    public class AgencyProfile
    {
        [Key]
        public int AgencyProfileId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [MaxLength(100)]
        public string AgencyName { get; set; }

        [MaxLength(100)]
        public string InstagramHandle { get; set; }

        [MaxLength(100)]
        public string TwitterHandle { get; set; }

        [MaxLength(100)]
        public string WhatsappHandle { get; set; }

        [MaxLength(200)]
        public string Services { get; set; }

        [MaxLength(50)]
        public string City { get; set; }
    }
}
