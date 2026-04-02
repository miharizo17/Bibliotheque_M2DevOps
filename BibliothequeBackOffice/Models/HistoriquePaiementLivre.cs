using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class HistoriquePaiementLivre
{
    public int Id { get; set; }
    public DateTime? Date_Lecture { get; set; }
    public int Id_Livre { get; set; }
    public int Id_Utilisateur { get; set; }
    public double Prix { get; set; }

    // Navigation
    public Livre? Livre { get; set; }
    public Utilisateur? Utilisateur { get; set; }
}
}