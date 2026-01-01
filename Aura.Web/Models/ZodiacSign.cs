namespace Aura.Web.Models
{
    public class ZodiacSign
    {
        public int ZodiacSignId { get; set; }
        public string Name { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }
        public string Symbol { get; set; }
    }
}
