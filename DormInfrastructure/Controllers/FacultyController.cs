using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacultyController : Controller
    {
        private readonly DbDormContext _context;

        public FacultyController(DbDormContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Faculties.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var faculty = await _context.Faculties
                .Include(f => f.Departments)
                .Include(f => f.Students)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string FaName, string? FaInformation,
            string? FaTelefon, string? FaKorpus, string? FaDekan)
        {
            var faculty = new Faculty
            {
                FaName = FaName,
                FaInformation = FaInformation,
                FaTelefon = FaTelefon,
                FaKorpus = FaKorpus,
                FaDekan = FaDekan
            };
            _context.Add(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            string FaName, string? FaInformation,
            string? FaTelefon, string? FaKorpus, string? FaDekan)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null) return NotFound();

            faculty.FaName = FaName;
            faculty.FaInformation = FaInformation;
            faculty.FaTelefon = FaTelefon;
            faculty.FaKorpus = FaKorpus;
            faculty.FaDekan = FaDekan;

            try
            {
                _context.Update(faculty);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Faculties.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty != null)
            {
                _context.Faculties.Remove(faculty);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}