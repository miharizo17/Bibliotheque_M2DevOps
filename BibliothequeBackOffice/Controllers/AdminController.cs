using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryBackOffice.Data;
using LibraryBackOffice.Models;

namespace LibraryBackOffice.Controllers
{
    public class AdminController : Controller
    {
        private readonly LibraryContext _context;

        public AdminController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Admin
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Admin
        [HttpPost]
        public IActionResult Index(string Username, string Password)
        {
            try
            {
                var Admin = _context.Admin.FirstOrDefault(u => u.Username == Username && u.Password == Password);
                if (Admin != null)
                {
                    HttpContext.Session.SetInt32("AdminId", Admin.Id!);
                    HttpContext.Session.SetString("Username", Admin.Username!);
                    HttpContext.Session.SetString("Nom", Admin.Nom!);
                    HttpContext.Session.SetString("Prenom", Admin.Prenom!);
                    return RedirectToAction("Index","Livre");
                }
                else
                {
                    ViewBag.Message = "Username ou Password incorrects";
                    ModelState.AddModelError(string.Empty, "Username ou Password incorrects");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Message = "Une erreur est survenu, veuillez réessayer plus tard.";
                ModelState.AddModelError(string.Empty, "Une erreur est survenu, veuillez réessayer plus tard.");
                return View();
            }
        }

        // GET: Admin/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/List
        public async Task<IActionResult> List()
        {
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Prenom,Username,Password,Active")] Admin adminModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(adminModel);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admin.FindAsync(id);
            if (adminModel == null)
            {
                return NotFound();
            }
            return View(adminModel);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Username,Password,Active")] Admin adminModel)
        {
            if (id != adminModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminModelExists(adminModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            return View(adminModel);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminModel = await _context.Admin.FindAsync(id);
            if (adminModel != null)
            {
                _context.Admin.Remove(adminModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool AdminModelExists(int id)
        {
            return _context.Admin.Any(e => e.Id == id);
        }
    }
}
