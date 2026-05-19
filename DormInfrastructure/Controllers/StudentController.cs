using DormDomain.Model;
using DormInfrastructure;
using DormInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly DbDormContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(DbDormContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Головна сторінка для студента
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentHome()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.FullName = user?.FullName ?? user?.Email;
            ViewBag.IsLinked = user?.StudentId != null;
            return View();
        }

        // Моє проживання
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyAccommodation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                ViewBag.Message = "Ваш акаунт не прив'язаний до жодного студента. Зверніться до адміністратора.";
                return View(new List<Accommodation>());
            }

            var accommodations = await _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St)
                .Where(a => a.StId == user.StudentId)
                .ToListAsync();

            return View(accommodations);
        }

        // Мої оплати (перегляд підтвердження оплат)
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyPayments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                ViewBag.Message = "Ваш акаунт не прив'язаний до жодного студента.";
                return View(new List<Accommodation>());
            }

            // Показуємо активне проживання як підтвердження оплати за гуртожиток
            var accommodations = await _context.Accommodations
                .Include(a => a.Ki).ThenInclude(r => r.Gu)
                .Include(a => a.St)
                .Where(a => a.StId == user.StudentId)
                .ToListAsync();

            return View(accommodations);
        }

        // Список всіх студентів — тільки Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var students = _context.Students
                .Include(s => s.Fa)
                .Include(s => s.Ka);
            return View(await students.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students
                .Include(s => s.Fa).Include(s => s.Ka)
                .Include(s => s.Accommodations).ThenInclude(a => a.Ki).ThenInclude(r => r.Gu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName");
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int FaId, int KaId, string StPib,
            int? StKurs, string? StTelefon, DateOnly? StDataNarodz)
        {
            var student = new Student
            {
                FaId = FaId,
                KaId = KaId,
                StPib = StPib,
                StKurs = StKurs,
                StTelefon = StTelefon,
                StDataNarodz = StDataNarodz
            };
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName", student.FaId);
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName", student.KaId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, int FaId, int KaId, string StPib,
            int? StKurs, string? StTelefon, DateOnly? StDataNarodz)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.FaId = FaId;
            student.KaId = KaId;
            student.StPib = StPib;
            student.StKurs = StKurs;
            student.StTelefon = StTelefon;
            student.StDataNarodz = StDataNarodz;

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students
                .Include(s => s.Fa).Include(s => s.Ka)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Relocate(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.StudentName = student.StPib;
            ViewData["KiId"] = new SelectList(
                _context.Rooms.Include(r => r.Gu).Select(r => new
                {
                    r.Id,
                    Display = "Гурт. №" + r.Gu.GuNomer + " / Кімн. №" + r.KiNomer
                }), "Id", "Display");
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Relocate(int id, int newRoomId)
        {
            var current = await _context.Accommodations
                .Where(a => a.StId == id && a.PrDataVysel == null)
                .FirstOrDefaultAsync();

            if (current != null)
                current.PrDataVysel = DateOnly.FromDateTime(DateTime.Today);

            _context.Accommodations.Add(new Accommodation
            {
                StId = id,
                KiId = newRoomId,
                PrDataZasel = DateOnly.FromDateTime(DateTime.Today)
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Evict(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students
                .Include(s => s.Accommodations).ThenInclude(a => a.Ki)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Evict")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EvictConfirmed(int id)
        {
            var current = await _context.Accommodations
                .Where(a => a.StId == id && a.PrDataVysel == null)
                .FirstOrDefaultAsync();

            if (current != null)
            {
                current.PrDataVysel = DateOnly.FromDateTime(DateTime.Today);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}