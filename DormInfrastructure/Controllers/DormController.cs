using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DormController : Controller
    {
        private readonly DbDormContext _context;

        public DormController(DbDormContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Dorms.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var dorm = await _context.Dorms
                .Include(d => d.Rooms)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dorm == null) return NotFound();
            return View(dorm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string GuNomer, string? GuAdresa,
            int? GuKilkistPoverh, string? GuKomendant, string? GuInformation)
        {
            var dorm = new Dorm
            {
                GuNomer = GuNomer,
                GuAdresa = GuAdresa,
                GuKilkistPoverh = GuKilkistPoverh,
                GuKomendant = GuKomendant,
                GuInformation = GuInformation
            };
            _context.Add(dorm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dorm = await _context.Dorms.FindAsync(id);
            if (dorm == null) return NotFound();
            return View(dorm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            string GuNomer, string? GuAdresa,
            int? GuKilkistPoverh, string? GuKomendant, string? GuInformation)
        {
            var dorm = await _context.Dorms.FindAsync(id);
            if (dorm == null) return NotFound();

            dorm.GuNomer = GuNomer;
            dorm.GuAdresa = GuAdresa;
            dorm.GuKilkistPoverh = GuKilkistPoverh;
            dorm.GuKomendant = GuKomendant;
            dorm.GuInformation = GuInformation;

            try
            {
                _context.Update(dorm);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Dorms.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dorm = await _context.Dorms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dorm == null) return NotFound();
            return View(dorm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dorm = await _context.Dorms.FindAsync(id);
            if (dorm != null)
            {
                _context.Dorms.Remove(dorm);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}