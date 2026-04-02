using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryBackOffice.Data;
using LibraryBackOffice.Models;
using CsvHelper;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http.Connections;
using LibraryBackOffice.Services;

namespace LibraryBackOffice.Controllers
{
    public class LivreController : BaseController
    {
        private readonly LibraryContext _context;
        private readonly ImportService _importService;

        public LivreController(LibraryContext context, ImportService importService)
        {
            _context = context;
            _importService = importService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 4, string? search = null, int? idtypelivre = null)
        {
            if(page<1) page=1;
            
            var query = _context.Livres
                .OrderBy(l => l.Date_Edition)
                .AsNoTracking();

            if(search != null)
            {
                query = query.Where(l => l.Titre!.Contains(search) || l.Auteur!.Contains(search));
            }
            if(idtypelivre.HasValue)
            {
                query = query.Where(l => l.Id_TypeLivre == idtypelivre.Value);
            }
            
            var totalItems = await query.CountAsync();

            var livres = await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var model = new PageResults<Livre>
            {
                Items = livres,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        public IActionResult Create()
        {
            var typesLivre = _context.TypeLivres.ToList();
            ViewBag.TypeLivreList = new SelectList(typesLivre,"Id","Type_Livre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdTypeLivre,Titre,SousTitre,Saison,Auteur,DateEdition,Description,Image,Document")] Livre livre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(livre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }
            var typesLivre = _context.TypeLivres.ToList();
            ViewBag.TypeLivreList = new SelectList(typesLivre,"Id","Type_Livre");
            return View(livre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdTypeLivre,Titre,SousTitre,Saison,Auteur,DateEdition,Description,Image,Document")] Livre livre)
        {
            if (id != livre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivreExists(livre.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        // GET: Livre/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }
            var typesLivre = _context.TypeLivres.ToList();
            ViewBag.TypeLivreList = new SelectList(typesLivre,"Id","Type_Livre");
            return View(livre);
        }

        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre != null)
            {
                _context.Livres.Remove(livre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult ImportCsv()
        {
            var model = new ImportCsvViewModel();
            
            // Récupérer le résultat depuis TempData si présent (après dry-run ou confirm)
            if (TempData["ImportResult"] is string jsonResult)
            {
                model.Result = System.Text.Json.JsonSerializer.Deserialize<ImportResult>(jsonResult);
                model.HasResult = true;
            }

            if (TempData["Success"] is string successMsg)
                model.SuccessMessage = successMsg;

            if (TempData["Error"] is string errorMsg)
                model.ErrorMessage = errorMsg;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError("", "Fichier CSV invalide.");
                return View();
            }

            List<Livre> livres;

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<LivreCsvMap>();

                try
                {
                    livres = csv.GetRecords<Livre>().ToList();
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", $"Erreur CSV : {ex.Message}");
                    return View();
                }
            }

            foreach(var livre in livres)
            {
                if (String.IsNullOrWhiteSpace(livre.Titre))
                {
                    ModelState.AddModelError("","Un livre a un titre vide");
                    return View();
                }
                
                bool typeExist = await _context.TypeLivres.AnyAsync(t => t.Id == livre.Id_TypeLivre);
                if (!typeExist)
                {
                    ModelState.AddModelError("",$"Type de livre inexistant (IdTypeLivre={livre.Id_TypeLivre})");
                    return View();
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Livres.AddRange(livres);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("",$"Erreur lors de l'import. Aucune donnée enregistrée. {ex.Message}");
                return View();
            }

            TempData["Success"] = $"{livres.Count} livre(s) importé(s) avec success.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsvDryRun(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Veuillez sélectionner un fichier CSV valide.";
                return RedirectToAction(nameof(ImportCsv));
            }

            try
            {
                var result = await _importService.ProcessImportAsync(csvFile, isDryRun: true);
                
                // Sérialiser le résultat pour le passer via TempData
                var json = System.Text.Json.JsonSerializer.Serialize(result);
                TempData["ImportResult"] = json;
                
                TempData["Info"] = $"Validation terminée : {result.ValidRows}/{result.TotalRows} ligne(s) valide(s).";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la validation : {ex.Message}";
            }

            return RedirectToAction(nameof(ImportCsv));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsvConfirm(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Veuillez sélectionner un fichier CSV valide.";
                return RedirectToAction(nameof(ImportCsv));
            }

            try
            {
                var result = await _importService.ProcessImportAsync(csvFile, isDryRun: false);

                if (string.IsNullOrEmpty(result.ErrorMessage))
                {
                    TempData["Success"] = $"{result.InsertedRows} livre(s) importé(s) avec succès sur {result.ValidRows} ligne(s) valide(s).";
                }
                else
                {
                    TempData["Error"] = result.ErrorMessage;
                }

                // Optionnel : repasser le résultat détaillé
                var json = System.Text.Json.JsonSerializer.Serialize(result);
                TempData["ImportResult"] = json;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de l'import : {ex.Message}";
            }

            return RedirectToAction(nameof(ImportCsv));
        }
    }
}
