using Microsoft.AspNetCore.Mvc;
using FrontBibliotheque.Data;

namespace FrontBibliotheque.Controllers.Api
{
    [ApiController]
    [Route("api/historique-lecture")]
    public class V_historiquelectureApiController : ControllerBase
    {
        private readonly V_historiquelectureRepository _repo;

        public V_historiquelectureApiController(V_historiquelectureRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? titre,
            [FromQuery] DateTime? dateDebut,
            [FromQuery] DateTime? dateFin)
        {
            int? id = HttpContext.Session.GetInt32("id");

            if (id == null)
                return Unauthorized(new { message = "Utilisateur non connecte" });

            var data = _repo.GetByUtilisateur(id.Value, titre, dateDebut, dateFin);
            return Ok(data);
        }
    }
}
