namespace FrontBibliotheque.Models;

public class V_historiquelecture
{
    public int id { get; set; }
    public DateTime? date_lecture { get; set; }
    public int? id_livre { get; set; }
    public int? id_utilisateur { get; set; }
    public string? titre { get; set; }
    public string? auteur { get; set; }
    public string? image { get; set; }
    public string? document { get; set; }
}
