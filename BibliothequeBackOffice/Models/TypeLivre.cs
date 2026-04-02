using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class TypeLivre
    {
        public int Id {get; set;}
        public string? Type_Livre {get; set;}

        public ICollection<Livre>? Livres {get; set;}
    }
}