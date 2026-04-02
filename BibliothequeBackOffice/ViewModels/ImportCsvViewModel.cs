using LibraryBackOffice.Models;
using LibraryBackOffice.Services;

namespace LibraryBackOffice.Models
{
    public class ImportCsvViewModel
    {
        public bool HasResult { get; set; }
        public ImportResult? Result { get; set; }

        // Pour afficher un résumé simple dans la vue
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public ImportCsvViewModel()
        {
            Result = new ImportResult();
        }
    }
}