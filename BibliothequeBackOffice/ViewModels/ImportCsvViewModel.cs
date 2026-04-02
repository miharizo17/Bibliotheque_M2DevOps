using LibraryBackOffice.Models;
using LibraryBackOffice.Services;

namespace LibraryBackOffice.Models
{
    public class ImportCsvViewModel
    {
        public bool HasResult { get; set; }
        public ImportResult? Result { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public ImportCsvViewModel()
        {
            Result = new ImportResult();
        }
    }
}