using Microsoft.AspNetCore.Mvc;

namespace FrontBibliotheque.Controllers
{
    public class HistoriqueAbonnementController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/Abonnement/ListeAbonnement.cshtml");
        }
    }
}
