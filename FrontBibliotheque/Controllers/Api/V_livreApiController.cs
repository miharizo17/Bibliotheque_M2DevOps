using Microsoft.AspNetCore.Mvc;
using FrontBibliotheque.Data;
using FrontBibliotheque.Models;

namespace FrontBibliotheque.Controllers.Api
{
    [ApiController]
    [Route("api/livre")]
    public class V_livreApiController : ControllerBase
    {
        private readonly V_livreRepository _repo;

        public V_livreApiController(V_livreRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int? idtypelivre,
            [FromQuery] string? autre,
            [FromQuery] bool? exclureLus)
        {
            int? idUtilisateur = null;

            if (exclureLus == true)
                idUtilisateur = HttpContext.Session.GetInt32("id");

            var livres = _repo.GetAll(idtypelivre, autre, idUtilisateur);
            return Ok(livres);
        }


        [HttpGet("typeLivre")]
        public IActionResult typeLivre()
        {
            return Ok(_repo.listeTypeLivre());
        }
     
    }
}
