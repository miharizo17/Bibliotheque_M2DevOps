using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class Livre
    {
        public int Id { get; set; }
        public int Id_TypeLivre { get; set; }
        public string? Titre { get; set; }
        public string? Sous_Titre { get; set; }
        public string? Saison { get; set; }
        public string? Auteur { get; set; }
        public DateTime? Date_Edition { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Document { get; set; }
        public int Etat { get; set; } = 0;

        // Navigation
        public TypeLivre? TypeLivre { get; set; }
        public ICollection<HistoriqueLecture>? HistoriqueLectures { get; set; }
        public ICollection<HistoriquePaiementLivre>? HistoriquePaiementLivres { get; set; }
    }
}