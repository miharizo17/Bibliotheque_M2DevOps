using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class ImportHistory
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public DateTime ImportDate { get; set; } = DateTime.UtcNow;

        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InsertedRows { get; set; }
        public int ErrorRows { get; set; }

        public bool IsDryRun { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? AdminId { get; set; }
    }
}