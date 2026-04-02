using Microsoft.AspNetCore.Mvc;

namespace FrontBibliotheque.Controllers
{
    public class HistoriqueLectureController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/HistoriqueLecture/Liste.cshtml");
        }
    }
}
