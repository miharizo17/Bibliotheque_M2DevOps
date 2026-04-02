using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryBackOffice.Data;
using LibraryBackOffice.Models;
using LibraryBackOffice.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace LibraryBackOffice.Controllers
{
    public class AbonnementController : BaseController
    {
        private readonly LibraryContext _context;

        public AbonnementController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1, int pageSize=10, int? mois=null, int? annee=null)
        {
            if(page<1) page = 1;
            
            var query = _context.HistoriqueAbonnements
                .Include(h => h.Utilisateur)
                .Include(h => h.TypeAbonnement)
                .Include(h => h.ModePaiement)
                .AsQueryable();

            if(mois.HasValue)
            {
                query = query.Where(h => h.Date_Paiement!.Value.Month == mois.Value);
            }
            if(annee.HasValue)
            {
                query = query.Where(h => h.Date_Paiement!.Value.Year == annee.Value);
            }

            query = query.OrderByDescending(h => h.Date_Paiement);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var model = new PageResults<HistoriqueAbonnement>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);
        }

        // GET: Abonnement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueAbonnement = await _context.HistoriqueAbonnements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiqueAbonnement == null)
            {
                return NotFound();
            }

            return View(historiqueAbonnement);
        }

        // GET: Abonnement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Abonnement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatePaiement,DateExpiration,IdTypeAbonnement,IdModePaiement,IdUtilisateur")] HistoriqueAbonnement historiqueAbonnement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historiqueAbonnement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(historiqueAbonnement);
        }

        // GET: Abonnement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueAbonnement = await _context.HistoriqueAbonnements.FindAsync(id);
            if (historiqueAbonnement == null)
            {
                return NotFound();
            }
            return View(historiqueAbonnement);
        }

        // POST: Abonnement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatePaiement,DateExpiration,IdTypeAbonnement,IdModePaiement,IdUtilisateur")] HistoriqueAbonnement historiqueAbonnement)
        {
            if (id != historiqueAbonnement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historiqueAbonnement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoriqueAbonnementExists(historiqueAbonnement.Id))
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
            return View(historiqueAbonnement);
        }

        // GET: Abonnement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueAbonnement = await _context.HistoriqueAbonnements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiqueAbonnement == null)
            {
                return NotFound();
            }

            return View(historiqueAbonnement);
        }

        // POST: Abonnement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historiqueAbonnement = await _context.HistoriqueAbonnements.FindAsync(id);
            if (historiqueAbonnement != null)
            {
                _context.HistoriqueAbonnements.Remove(historiqueAbonnement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoriqueAbonnementExists(int id)
        {
            return _context.HistoriqueAbonnements.Any(e => e.Id == id);
        }
        public async Task<IActionResult> ExportPdf(int? mois = null, int? annee = null)
        {
            if (!mois.HasValue) mois = DateTime.Now.Month;
            if (!annee.HasValue) annee = DateTime.Now.Year;

            QuestPDF.Settings.License = LicenseType.Community;

            var data = await _context.HistoriqueAbonnements
                .Include(h => h.Utilisateur)
                .Include(h => h.TypeAbonnement)
                .Where(h => h.Date_Paiement!.Value.Month == mois && h.Date_Paiement!.Value.Year == annee)
                .OrderByDescending(h => h.Date_Paiement)
                .Select(h => new AbonnementPdfDto
                {
                    DatePaiement = h.Date_Paiement!.Value,
                    Utilisateur = h.Utilisateur!.Nom + " " + h.Utilisateur.Prenom,
                    TypeAbonnement = h.TypeAbonnement!.Type_Abonnement,
                    DateExpiration = h.Date_Expiration!.Value
                })
                .AsNoTracking()
                .ToListAsync();

            if (!data.Any())
                return BadRequest("Aucune donnée à exporter.");

            var document = new LibraryBackOffice.Documents.AbonnementPdfDocument(data, mois.Value, annee.Value);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", $"abonnements_{mois:D2}_{annee}.pdf");
        }
    }
}
