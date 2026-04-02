using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class ModePaiement
    {
        public int Id {get; set;}
        public string? Mode {get; set;}

        public ICollection<HistoriqueAbonnement>? HistoriqueAbonnements {get; set;}

    }
}