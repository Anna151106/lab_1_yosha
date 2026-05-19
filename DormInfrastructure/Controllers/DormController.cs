using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DormInfrastructure.Controllers
{
    [Authorize]
    public class DormController : Controller
    {
        private readonly DbDormContext _context;

        public DormController(DbDormContext context)
        {
            _context = context;
        }

        // GET: Dorm
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dorms.ToListAsync());
        }

        // GET: Dorm/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dorm = await _context.Dorms
                .Include(d => d.Rooms)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dorm == null) return NotFound();

            return View(dorm);
        }

        // GET: Dorm/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dorm/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GuNomer,GuAdresa,GuKilkistPoverh,GuKomendant,GuInformation")] Dorm dorm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dorm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dorm);
        }

        // GET: Dorm/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dorm = await _context.Dorms.FindAsync(id);
            if (dorm == null) return NotFound();

            return View(dorm);
        }

        // POST: Dorm/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,GuNomer,GuAdresa,GuKilkistPoverh,GuKomendant,GuInformation")] Dorm dorm)
        {
            if (id != dorm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dorm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Dorms.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dorm);
        }

        // GET: Dorm/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dorm = await _context.Dorms
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dorm == null) return NotFound();

            return View(dorm);
        }

        // POST: Dorm/Delete/5
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