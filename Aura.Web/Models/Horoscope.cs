using System;

namespace Aura.Web.Models
{
    public class Horoscope
    {
        public int HoroscopeId { get; set; }
        public int ZodiacId { get; set; }
        public int TypeId { get; set; }
        public DateTime HoroscopeDate { get; set; }
        public string PredictionText { get; set; }
    }
}
