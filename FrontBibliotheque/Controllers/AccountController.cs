using Microsoft.AspNetCore.Mvc;

namespace FrontBibliotheque.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
