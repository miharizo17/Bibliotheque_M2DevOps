namespace FrontBibliotheque.Models;

public class V_historiqueAbonnementModel
{
        public int id { get; set; }
        public DateTime? date_paiement { get; set; }
        public int? id_typeabonnement { get; set; }
        public int? id_modepaiement { get; set; }
        public int? id_utilisateur { get; set; }
        public DateTime? date_expiration { get; set; }
        public string type_abonnement { get; set; }
        public string mode { get; set; }
}
