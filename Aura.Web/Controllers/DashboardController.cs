using Microsoft.AspNetCore.Mvc;

namespace Aura.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
