namespace FrontBibliotheque.Models;

public class HistoriqueAbonnementModel
{
        public int id { get; set; }
        public DateTime? date_paiement { get; set; }
        public int? id_typeabonnement { get; set; }
        public int? id_modepaiement { get; set; }
        public int? id_utilisateur { get; set; }
        public DateTime? date_expiration { get; set; }
}
