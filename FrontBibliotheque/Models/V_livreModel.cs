namespace FrontBibliotheque.Models;

public class V_livreModel
{
        public int id { get; set; }
        public int? id_typelivre { get; set; }
        public string? titre { get; set; }
        public string? sous_titre { get; set; }
        public string? saison { get; set; }
        public string? auteur { get; set; }
        public DateTime? date_edition {get; set;}
        public string? description { get; set; }
        public string? image { get; set; }
        public string? document { get; set; }
        public int? etat { get; set; }
        public string? type_livre { get; set; }
}