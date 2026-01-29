using System.ComponentModel.DataAnnotations;

namespace aura_assist_prod.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } // "User", "Influencer", "Agency"

        [MaxLength(50)]
        public string City { get; set; }
    }
}
