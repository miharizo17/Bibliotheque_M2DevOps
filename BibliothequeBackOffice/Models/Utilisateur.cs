using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Telephone { get; set; }
        public string? Mail { get; set; }
        public string? Mdp { get; set; }
        public int Etat { get; set; } = 0;
        public DateTime? DateEntree { get; set; }

        public ICollection<HistoriqueAbonnement>? HistoriqueAbonnements { get; set; }
        public ICollection<HistoriqueLecture>? HistoriqueLectures { get; set; }
        public ICollection<HistoriquePaiementLivre>? HistoriquePaiementLivres { get; set; }
    }
}