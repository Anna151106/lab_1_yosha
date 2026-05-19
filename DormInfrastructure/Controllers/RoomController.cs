using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DormInfrastructure.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly DbDormContext _context;

        public RoomController(DbDormContext context)
        {
            _context = context;
        }

        // GET: Room
        public async Task<IActionResult> Index()
        {
            var rooms = _context.Rooms.Include(r => r.Gu);
            return View(await rooms.ToListAsync());
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Gu)
                .Include(r => r.Accommodations)
                    .ThenInclude(a => a.St)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }

        // GET: Room/Create
        public IActionResult Create()
        {
            ViewData["GuId"] = new SelectList(_context.Dorms, "Id", "GuNomer");
            return View();
        }

        // POST: Room/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GuId,KiNomer,KiMistkist,KiPoverh,KiInformation")] Room room)
        {
            // ВИПРАВЛЕННЯ: Ігноруємо об'єкт гуртожитку при перевірці форми кімнати
            ModelState.Remove("Gu");

            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuId"] = new SelectList(_context.Dorms, "Id", "GuNomer", room.GuId);
            return View(room);
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            ViewData["GuId"] = new SelectList(_context.Dorms, "Id", "GuNomer", room.GuId);
            return View(room);
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,GuId,KiNomer,KiMistkist,KiPoverh,KiInformation")] Room room)
        {
            if (id != room.Id) return NotFound();

            // ВИПРАВЛЕННЯ: Ігноруємо об'єкт гуртожитку при редагуванні
            ModelState.Remove("Gu");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuId"] = new SelectList(_context.Dorms, "Id", "GuNomer", room.GuId);
            return View(room);
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Gu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Room/Delete/5
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