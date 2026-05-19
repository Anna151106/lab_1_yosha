using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly DbDormContext _context;

        public DepartmentController(DbDormContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var departments = _context.Departments.Include(d => d.Fa);
            return View(await departments.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var department = await _context.Departments
                .Include(d => d.Fa)
                .Include(d => d.Students)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null) return NotFound();
            return View(department);
        }

        public IActionResult Create()
        {
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int FaId, string KaName, string? KaInformation,
            string? KaTelefon, string? KaZaviduvach)
        {
            var department = new Department
            {
                FaId = FaId,
                KaName = KaName,
                KaInformation = KaInformation,
                KaTelefon = KaTelefon,
                KaZaviduvach = KaZaviduvach
            };
            _context.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName", department.FaId);
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            int FaId, string KaName, string? KaInformation,
            string? KaTelefon, string? KaZaviduvach)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            department.FaId = FaId;
            department.KaName = KaName;
            department.KaInformation = KaInformation;
            department.KaTelefon = KaTelefon;
            department.KaZaviduvach = KaZaviduvach;

            try
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Departments.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var department = await _context.Departments
                .Include(d => d.Fa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}