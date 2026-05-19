using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    public class AccommodationController : Controller
    {
        private readonly DbDormContext _context;

        public AccommodationController(DbDormContext context)
        {
            _context = context;
        }

        // GET: Accommodation
        public async Task<IActionResult> Index()
        {
            var accommodations = _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St);
            return View(await accommodations.ToListAsync());
        }

        // GET: Accommodation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var accommodation = await _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (accommodation == null) return NotFound();

            return View(accommodation);
        }

        // GET: Accommodation/Create
        public IActionResult Create()
        {
            ViewData["KiId"] = new SelectList(_context.Rooms, "Id", "KiNomer");
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib");
            return View();
        }

        // POST: Accommodation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("KiId,StId,PrDataZasel,PrDataVysel")] Accommodation accommodation)
        {
            // ВИПРАВЛЕННЯ: Ігноруємо навігаційні об'єкти Кімнати та Студента при валідації заселення
            ModelState.Remove("Ki");
            ModelState.Remove("St");

            if (ModelState.IsValid)
            {
                _context.Add(accommodation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KiId"] = new SelectList(_context.Rooms, "Id", "KiNomer", accommodation.KiId);
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib", accommodation.StId);
            return View(accommodation);
        }

        // GET: Accommodation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null) return NotFound();

            ViewData["KiId"] = new SelectList(_context.Rooms, "Id", "KiNomer", accommodation.KiId);
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib", accommodation.StId);
            return View(accommodation);
        }

        // POST: Accommodation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,KiId,StId,PrDataZasel,PrDataVysel")] Accommodation accommodation)
        {
            if (id != accommodation.Id) return NotFound();

            // ВИПРАВЛЕННЯ: Ігноруємо навігаційні об'єкти при редагуванні
            ModelState.Remove("Ki");
            ModelState.Remove("St");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accommodation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Accommodations.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KiId"] = new SelectList(_context.Rooms, "Id", "KiNomer", accommodation.KiId);
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib", accommodation.StId);
            return View(accommodation);
        }

        // GET: Accommodation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var accommodation = await _context.Accommodations
                .Include(a => a.Ki)
                .Include(a => a.St)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (accommodation == null) return NotFound();

            return View(accommodation);
        }

        // POST: Accommodation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation != null)
            {
                _context.Accommodations.Remove(accommodation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}