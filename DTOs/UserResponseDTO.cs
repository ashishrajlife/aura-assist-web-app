namespace aura_assist_prod.DTOs
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool IsApproved { get; set; }
        public string City { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Token { get; set; }
    }
}
