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

namespace LibraryBackOffice.Controllers
{
    public class LectureController : BaseController
    {
        private readonly LibraryContext _context;

        public LectureController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Lecture
        public async Task<IActionResult> Index(int page=1,int pageSize=10,int? mois=null,int? annee=null)
        {
            if(page<1) page=1;

            var query = _context.HistoriqueLectures
                .Include(h => h.Livre)
                .Include(h => h.Utilisateur)
                .AsQueryable();
            
            if(mois.HasValue)
            {
                query = query.Where(h => h.Date_Lecture!.Value.Month == mois.Value);
            }
            if(annee.HasValue)
            {
                query = query.Where(h => h.Date_Lecture!.Value.Year == annee.Value);
            }

            query = query.OrderByDescending(h => h.Date_Lecture);
            
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            
            var model = new PageResults<HistoriqueLecture>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);
        }

        // GET: Lecture/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueLecture = await _context.HistoriqueLectures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiqueLecture == null)
            {
                return NotFound();
            }

            return View(historiqueLecture);
        }

        // GET: Lecture/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lecture/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateLecture,IdLivre,IdUtilisateur")] HistoriqueLecture historiqueLecture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historiqueLecture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(historiqueLecture);
        }

        // GET: Lecture/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueLecture = await _context.HistoriqueLectures.FindAsync(id);
            if (historiqueLecture == null)
            {
                return NotFound();
            }
            return View(historiqueLecture);
        }

        // POST: Lecture/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateLecture,IdLivre,IdUtilisateur")] HistoriqueLecture historiqueLecture)
        {
            if (id != historiqueLecture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historiqueLecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoriqueLectureExists(historiqueLecture.Id))
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
            return View(historiqueLecture);
        }

        // GET: Lecture/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiqueLecture = await _context.HistoriqueLectures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiqueLecture == null)
            {
                return NotFound();
            }

            return View(historiqueLecture);
        }

        // POST: Lecture/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historiqueLecture = await _context.HistoriqueLectures.FindAsync(id);
            if (historiqueLecture != null)
            {
                _context.HistoriqueLectures.Remove(historiqueLecture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoriqueLectureExists(int id)
        {
            return _context.HistoriqueLectures.Any(e => e.Id == id);
        }
    }
}
