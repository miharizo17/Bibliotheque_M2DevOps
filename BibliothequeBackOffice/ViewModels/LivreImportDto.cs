using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class LivreImportDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(255)]
        public string Titre { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Sous_Titre { get; set; }

        [MaxLength(100)]
        public string? Saison { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire")]
        [MaxLength(255)]
        public string Auteur { get; set; } = string.Empty;

        public DateTime? Date_Edition { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [MaxLength(500)]
        public string? Document { get; set; }

        [Required(ErrorMessage = "Le type de livre est obligatoire")]
        public string TypeLivreNom { get; set; } = string.Empty;

        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();
        public int? ResolvedTypeLivreId { get; set; }
        public bool IsDuplicateInFile { get; set; }
        public bool IsDuplicateInDatabase { get; set; }
    }
}