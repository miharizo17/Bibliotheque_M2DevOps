using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.ViewModels
{
    public class AbonnementPdfDto
    {
        public DateTime DatePaiement {get; set;}
        public string? Utilisateur {get; set;}
        public string? TypeAbonnement {get; set;}
        public DateTime DateExpiration {get; set;}
    }
}