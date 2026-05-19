using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccommodationController : Controller
    {
        private readonly DbDormContext _context;

        public AccommodationController(DbDormContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var accommodations = _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St);
            return View(await accommodations.ToListAsync());
        }

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

        public IActionResult Create()
        {
            ViewData["KiId"] = new SelectList(
                _context.Rooms.Include(r => r.Gu).Select(r => new
                {
                    r.Id,
                    Display = "Гурт. №" + r.Gu.GuNomer + " / Кімн. №" + r.KiNomer
                }), "Id", "Display");
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int KiId, int StId,
            DateOnly PrDataZasel, DateOnly? PrDataVysel)
        {
            var accommodation = new Accommodation
            {
                KiId = KiId,
                StId = StId,
                PrDataZasel = PrDataZasel,
                PrDataVysel = PrDataVysel
            };
            _context.Add(accommodation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null) return NotFound();
            ViewData["KiId"] = new SelectList(
                _context.Rooms.Include(r => r.Gu).Select(r => new
                {
                    r.Id,
                    Display = "Гурт. №" + r.Gu.GuNomer + " / Кімн. №" + r.KiNomer
                }), "Id", "Display", accommodation.KiId);
            ViewData["StId"] = new SelectList(_context.Students, "Id", "StPib", accommodation.StId);
            return View(accommodation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            int KiId, int StId,
            DateOnly PrDataZasel, DateOnly? PrDataVysel)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null) return NotFound();

            accommodation.KiId = KiId;
            accommodation.StId = StId;
            accommodation.PrDataZasel = PrDataZasel;
            accommodation.PrDataVysel = PrDataVysel;

            try
            {
                _context.Update(accommodation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Accommodations.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var accommodation = await _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accommodation == null) return NotFound();
            return View(accommodation);
        }

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