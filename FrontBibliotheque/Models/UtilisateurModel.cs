namespace FrontBibliotheque.Models;

public class UtilisateurModel
{
        public int id { get; set; }
        public string? nom { get; set; }
        public string? prenom { get; set; }
        public string? saison { get; set; }
        public string? telephone { get; set; }
        public string? mail { get; set; }
        public string? mdp { get; set; }
        public int? etat { get; set; }
        public DateTime? dateentree {get; set;}
}
