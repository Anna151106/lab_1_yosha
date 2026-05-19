using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomController : Controller
    {
        private readonly DbDormContext _context;

        public RoomController(DbDormContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = _context.Rooms.Include(r => r.Gu);
            return View(await rooms.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.Rooms
                .Include(r => r.Gu)
                .Include(r => r.Accommodations).ThenInclude(a => a.St)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null) return NotFound();
            return View(room);
        }

        public IActionResult Create()
        {
            ViewData["GuId"] = new SelectList(
                _context.Dorms.Select(d => new { d.Id, Display = "Гуртожиток №" + d.GuNomer }),
                "Id", "Display");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int GuId, string KiNomer, int? KiPoverh,
            int? KiMistkist, string? KiInformation)
        {
            var room = new Room
            {
                GuId = GuId,
                KiNomer = KiNomer,
                KiPoverh = KiPoverh,
                KiMistkist = KiMistkist,
                KiInformation = KiInformation
            };
            _context.Add(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            ViewData["GuId"] = new SelectList(
                _context.Dorms.Select(d => new { d.Id, Display = "Гуртожиток №" + d.GuNomer }),
                "Id", "Display", room.GuId);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            int GuId, string KiNomer, int? KiPoverh,
            int? KiMistkist, string? KiInformation)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.GuId = GuId;
            room.KiNomer = KiNomer;
            room.KiPoverh = KiPoverh;
            room.KiMistkist = KiMistkist;
            room.KiInformation = KiInformation;

            try
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Rooms.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.Rooms
                .Include(r => r.Gu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}