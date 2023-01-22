using Microsoft.AspNetCore.Mvc;

namespace PustokProj.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "Superadmin,Admin")]
	public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
