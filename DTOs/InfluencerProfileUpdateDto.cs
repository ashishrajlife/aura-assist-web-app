namespace aura_assist_prod.DTOs
{
    public class InfluencerProfileUpdateDto
    {
        public string InstagramHandle { get; set; }
        public string TwitterHandle { get; set; }
        public string WhatsappHandle { get; set; }
        public int FollowersCount { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
    }
}
