using Microsoft.AspNetCore.Mvc;
using FrontBibliotheque.Data;
using FrontBibliotheque.Models;

namespace FrontBibliotheque.Controllers.Api
{
    [ApiController]
    [Route("api/utilisateurs")]
    public class UtilisateurApiController : ControllerBase
    {
        private readonly UtilisateurRepository _repo;

        public UtilisateurApiController(UtilisateurRepository repo)
        {
            _repo = repo;
        }

        public class LoginRequest
        {
            public string mail { get; set; }
            public string mdp { get; set; }
        }


        // ✅ LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _repo.Login(request.mail, request.mdp);

            if (user != null)
            {
                HttpContext.Session.SetInt32("id", user.id);
                HttpContext.Session.SetString("nom", user.nom);
                HttpContext.Session.SetString("mail", user.mail);

                return Ok(new { message = "1" });
            }

            return Ok(new { message = "0" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // 🔥 efface toute la session
            return Ok(new { message = "Déconnexion réussie" });
        }


        [HttpGet("profile")]
        public IActionResult Profile()
        {
            int? id = HttpContext.Session.GetInt32("id");
            string? nom = HttpContext.Session.GetString("nom");
            string? mail = HttpContext.Session.GetString("mail");

            if (id == null)
            {
                return Unauthorized(new { message = "Non connecté" });
            }

            return Ok(new
            {
                id = id,
                nom = nom,
                mail = mail
            });
        }


        // ✅ GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repo.GetAll());
        }

        // ✅ GET BY ID
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Create([FromBody] UtilisateurModel u)
        {
            _repo.Add(u);
            return Ok();
        }

        // ✅ UPDATE
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UtilisateurModel u)
        {
            u.id = id;
            _repo.Update(u);
            return Ok();
        }

        // ✅ DELETE
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            return Ok();
        }
    }
}
