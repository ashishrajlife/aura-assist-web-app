using Microsoft.AspNetCore.Mvc;
using Aura.Web.Models.ViewModels;

namespace Aura.Web.Controllers
{
    public class KundliController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View(new KundliInputVM());
        }

        [HttpPost]
        public IActionResult Create(KundliInputVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔮 TEMP: This will later call real KundliService
            var result = KundliOutputVM.CreateEmpty(model);

            return View("Result", result);
        }
    }
}
