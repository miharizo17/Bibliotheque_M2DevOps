using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class Admin
    {
        public int Id {get; set;}
        public string? Nom {get; set;}
        public string? Prenom {get; set;}
        public string? Username {get; set;}
        public string? Password {get; set;}
        public bool Active {get; set;} = true;

    }
}