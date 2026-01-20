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
        public IActionResult GetAll([FromQuery] int? idtypelivre, [FromQuery] string? autre)
        {
            var livres = _repo.GetAll(idtypelivre, autre);

            return Ok(livres);
        }


        [HttpGet("typeLivre")]
        public IActionResult typeLivre()
        {
            return Ok(_repo.listeTypeLivre());
        }
     
    }
}
