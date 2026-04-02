using Microsoft.AspNetCore.Mvc;
using FrontBibliotheque.Data;
using FrontBibliotheque.Models;

namespace FrontBibliotheque.Controllers.Api
{
    [ApiController]
    [Route("api/abonnement")]
    public class HistoriqueAbonnementApiController : ControllerBase
    {
        private readonly HistoriqueAbonnementRepository _repo;

        public HistoriqueAbonnementApiController(HistoriqueAbonnementRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("ajoutAbonnement")]
        public IActionResult AjouterAbonnement([FromBody] HistoriqueAbonnementModel historique)
        {
            if (historique == null)
                return BadRequest(new { message = "Données invalides" });

            // 🔐 Récupération de l'utilisateur depuis la session
            int? id = HttpContext.Session.GetInt32("id");

            if (id == null)
                return Unauthorized(new { message = "Utilisateur non connecté" });

            try
            {
                // ✅ Injection de l'id utilisateur
                historique.id_utilisateur = id.Value;

                int result = _repo.AjoutAbonnementUtilisateur(historique);

                if (result == 1)
                {
                    return Ok(new
                    {
                        status = 1,
                        message = "Abonnement déjà existant"
                    });
                }

                return Ok(new
                {
                    status = 0,
                    message = "Abonnement ajouté avec succès"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur serveur",
                    error = ex.Message
                });
            }
        }

        [HttpGet("typeAbonnement")]
        public IActionResult typeAbonnement()
        {
            return Ok(_repo.listeTypeAbonnement());
        }

        [HttpGet("modePaiement")]
        public IActionResult modePaiement()
        {
            return Ok(_repo.listeModePaiement());
        }

        [HttpGet("listeAbonnement")]
        public IActionResult ListeAbonnement()
        {
            int? id = HttpContext.Session.GetInt32("id");

            if (id == null)
                return Unauthorized("Utilisateur non connecté");

            var result = _repo.listeAbonnement(id.Value);
            return Ok(result);
        }

 
     
        [HttpGet("peut-lire/{idUtilisateur:int}/{idLivre:int}")]
        public async Task<IActionResult> PeutLireLivre(
            int idUtilisateur,
            int idLivre)
        {
            var result = await _repo.PeutLireLivreAsync(idUtilisateur, idLivre);

            return Ok(new
            {
                autorise = result.PeutLire,
                statut = result.Statut,
                message = result.Message
            });
        }

        public class LectureRequest
        {
            public int idLivre { get; set; }
        }

        // ✅ Vérifier accès + enregistrer la lecture
        [HttpPost("verifier-lecture")]
        public async Task<IActionResult> VerifierLecture([FromBody] LectureRequest req)
        {
            int? id = HttpContext.Session.GetInt32("id");

            if (id == null)
                return Ok(new { statut = "non_connecte", message = "Veuillez vous connecter" });

            var result = await _repo.PeutLireLivreAsync(id.Value, req.idLivre);

            if (!result.PeutLire)
                return Ok(new { statut = result.Statut, message = result.Message });

            // Insérer dans historiquelecture seulement si pas déjà lu
            if (!result.DejaLu)
                await _repo.InsererHistoriqueLectureAsync(id.Value, req.idLivre);

            return Ok(new { statut = "ok", message = result.Message });
        }

        // ✅ Payer le livre (2000 Ar) + enregistrer la lecture
        [HttpPost("payer-lecture")]
        public async Task<IActionResult> PayerLecture([FromBody] LectureRequest req)
        {
            int? id = HttpContext.Session.GetInt32("id");

            if (id == null)
                return Ok(new { statut = "non_connecte", message = "Veuillez vous connecter" });

            try
            {
                await _repo.InsererPaiementLivreAsync(id.Value, req.idLivre, 2000);
                return Ok(new { statut = "ok", message = "Paiement effectué avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du paiement", error = ex.Message });
            }
        }
    }
}
