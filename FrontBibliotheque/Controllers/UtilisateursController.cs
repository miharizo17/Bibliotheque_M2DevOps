using Microsoft.AspNetCore.Mvc;

namespace FrontBibliotheque.Controllers
{
    public class UtilisateursController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
