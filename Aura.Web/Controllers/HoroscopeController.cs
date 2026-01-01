using Microsoft.AspNetCore.Mvc;
using Aura.Web.Services.Interfaces;
using Aura.Web.Data;
using System;

namespace Aura.Web.Controllers
{
    public class HoroscopeController : Controller
    {
        private readonly IZodiacService _zodiacService;

        public HoroscopeController(IZodiacService zodiacService)
        {
            _zodiacService = zodiacService;
        }

        public IActionResult Daily(DateTime dob)  //http://localhost:5159/Horoscope/Daily?dob=1998-12-25
        {
            var zodiac = _zodiacService.GetZodiacByDOB(dob);
            return Content($"Your Zodiac Sign is {zodiac.Name}");
        }
    }
}
