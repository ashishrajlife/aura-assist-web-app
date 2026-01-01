using Microsoft.AspNetCore.Mvc;
using Aura.Web.Services.Interfaces;
using Aura.Web.Models.ViewModels;
using System;

namespace Aura.Web.Controllers
{
    public class HoroscopeController : Controller
    {
        private readonly IZodiacService _zodiacService;
        private readonly IHoroscopeService _horoscopeService;

        public HoroscopeController(
            IZodiacService zodiacService,
            IHoroscopeService horoscopeService)
        {
            _zodiacService = zodiacService;
            _horoscopeService = horoscopeService;
        }

        public IActionResult Daily(DateTime dob)
        {
            var zodiac = _zodiacService.GetZodiacByDOB(dob);

            var horoscope = _horoscopeService
                .GetDailyHoroscope(zodiac.ZodiacSignId, DateTime.Today);

            var vm = new DailyHoroscopeVM
            {
                ZodiacName = zodiac.Name,
                Symbol = zodiac.Symbol,
                Date = DateTime.Today,
                Prediction = horoscope?.PredictionText 
                             ?? "No horoscope available for today."
            };

            return View(vm);
        }
    }
}
