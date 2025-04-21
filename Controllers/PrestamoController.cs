using Microsoft.AspNetCore.Mvc;

namespace BiblioApp.Controllers
{
    public class PrestamoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
