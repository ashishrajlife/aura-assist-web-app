using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("User");
    }

    public IActionResult User()
    {
        return View("UserDashboard");
    }

    public IActionResult Admin()
    {
        return View("AdminDashboard");
    }
}
