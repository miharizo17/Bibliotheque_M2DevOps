using Microsoft.AspNetCore.Mvc;

namespace FrontBibliotheque.Controllers
{
    public class V_livreController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/Livre/Liste.cshtml");
        }
    }
}
