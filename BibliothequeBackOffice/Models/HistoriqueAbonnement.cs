using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class HistoriqueAbonnement
    {
        public int Id { get; set; }
        public DateTime? Date_Paiement { get; set; }
        public int Id_TypeAbonnement { get; set; }
        public int Id_ModePaiement { get; set; }
        public int Id_Utilisateur { get; set; }
        public DateTime? Date_Expiration { get; set; }

        public TypeAbonnement? TypeAbonnement { get; set; }
        public ModePaiement? ModePaiement { get; set; }
        public Utilisateur? Utilisateur { get; set; }
    }
}