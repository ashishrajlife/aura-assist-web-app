using System.ComponentModel.DataAnnotations;

namespace aura_assist_prod.Models
{
   public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } // "User", "Influencer", "Agency"

        public bool IsApproved { get; set; } = false;

        [MaxLength(50)]
        public string City { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public InfluencerProfile InfluencerProfile { get; set; }
        public AgencyProfile AgencyProfile { get; set; }
    }
}
