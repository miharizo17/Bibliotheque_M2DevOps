using CsvHelper;
using Microsoft.EntityFrameworkCore;
using LibraryBackOffice.Data;
using LibraryBackOffice.Models;
using System.Globalization;
using System.Text;

namespace LibraryBackOffice.Services
{
    public class ImportService
    {
        private readonly LibraryContext _context;

        public ImportService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<ImportResult> ProcessImportAsync(IFormFile csvFile, bool isDryRun = false)
        {
            var result = new ImportResult
            {
                FileName = csvFile.FileName,
                IsDryRun = isDryRun
            };

            var importDtos = new List<LivreImportDto>();

            // 1. Parsing
            using (var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.Configuration.BadDataFound = context =>
                {
                    Console.WriteLine($"Bad data found on row {context.Context.Parser.Row}: {context.RawRecord}");
                };
                csv.Context.RegisterClassMap<LivreImportMap>();
                importDtos = csv.GetRecords<LivreImportDto>().ToList();
            }

            result.TotalRows = importDtos.Count;

            var seenInFile = new HashSet<(string Titre, string Auteur)>(
                new TupleEqualityComparer<string, string>(StringComparer.OrdinalIgnoreCase)
            );

            var allTypeLivres = await _context.TypeLivres.ToDictionaryAsync(
                t => t.Type_Livre?.Trim().ToLower() ?? "",
                t => t.Id);

            for (int i = 0; i < importDtos.Count; i++)
            {
                var dto = importDtos[i];
                dto.Errors.Clear();
                dto.IsValid = true;

                // Validation champs obligatoires
                if (string.IsNullOrWhiteSpace(dto.Titre))
                {
                    dto.Errors.Add("Titre obligatoire");
                    dto.IsValid = false;
                }
                if (string.IsNullOrWhiteSpace(dto.Auteur))
                {
                    dto.Errors.Add("Auteur obligatoire");
                    dto.IsValid = false;
                }
                if (string.IsNullOrWhiteSpace(dto.TypeLivreNom))
                {
                    dto.Errors.Add("Type de livre obligatoire");
                    dto.IsValid = false;
                }

                // Résolution du TypeLivre par nom
                var typeKey = dto.TypeLivreNom.Trim().ToLower();
                if (allTypeLivres.TryGetValue(typeKey, out int typeId))
                {
                    dto.ResolvedTypeLivreId = typeId;
                }
                else
                {
                    dto.Errors.Add($"Type de livre non trouvé : {dto.TypeLivreNom}");
                    dto.IsValid = false;
                }

                // Détection doublons dans le fichier
                var key = (dto.Titre.Trim(), dto.Auteur.Trim());
                if (seenInFile.Contains(key))
                {
                    dto.IsDuplicateInFile = true;
                    dto.Errors.Add("Doublon dans le fichier (même titre + auteur)");
                    dto.IsValid = false;
                }
                else
                {
                    seenInFile.Add(key);
                }

                // Doublon dans la base
                bool existsInDb = await _context.Livres.AnyAsync(l =>
                    l.Titre == dto.Titre && l.Auteur == dto.Auteur);
                if (existsInDb)
                {
                    dto.IsDuplicateInDatabase = true;
                    dto.Errors.Add("Déjà existant en base (même titre + auteur)");
                    dto.IsValid = false;   // On peut décider de skipper ou d'autoriser selon besoin
                }
            }

            result.ValidRows = importDtos.Count(d => d.IsValid);
            result.ErrorRows = importDtos.Count(d => !d.IsValid);
            result.Items = importDtos;

            // 3. Si ce n'est pas un dry-run → insertion
            if (!isDryRun && result.ValidRows > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var livresToInsert = importDtos
                        .Where(d => d.IsValid)
                        .Select(dto => new Livre
                        {
                            Titre = dto.Titre,
                            Sous_Titre = dto.Sous_Titre,
                            Saison = dto.Saison,
                            Auteur = dto.Auteur,
                            Date_Edition = dto.Date_Edition,
                            Description = dto.Description,
                            Image = dto.Image,
                            Document = dto.Document,
                            Id_TypeLivre = dto.ResolvedTypeLivreId!.Value,
                            Etat = 0
                        })
                        .ToList();

                    _context.Livres.AddRange(livresToInsert);
                    await _context.SaveChangesAsync();

                    result.InsertedRows = livresToInsert.Count;

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    result.ErrorMessage = $"Erreur lors de l'insertion : {ex.Message}";
                    result.InsertedRows = 0;
                }
            }

            // 4. Enregistrement de l'historique
            var history = new ImportHistory
            {
                FileName = csvFile.FileName,
                TotalRows = result.TotalRows,
                ValidRows = result.ValidRows,
                InsertedRows = result.InsertedRows,
                ErrorRows = result.ErrorRows,
                IsDryRun = isDryRun,
                Notes = result.ErrorMessage ?? $"Import du {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
            };

            _context.ImportHistories.Add(history);
            await _context.SaveChangesAsync();

            return result;
        }
    }

    // Résultat global de l'import
    public class ImportResult
    {
        public string FileName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InsertedRows { get; set; }
        public int ErrorRows { get; set; }
        public bool IsDryRun { get; set; }
        public string? ErrorMessage { get; set; }
        public List<LivreImportDto> Items { get; set; } = new();
    }
}