using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class TypeAbonnement
    {
        public int Id {get; set;}
        public string? Type_Abonnement {get; set;}

        public ICollection<HistoriqueAbonnement>? HistoriqueAbonnements {get; set;}

    }
}