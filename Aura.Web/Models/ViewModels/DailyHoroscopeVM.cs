using System;

namespace Aura.Web.Models.ViewModels
{
    public class DailyHoroscopeVM
    {
        public string ZodiacName { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public string Prediction { get; set; }
    }
}
