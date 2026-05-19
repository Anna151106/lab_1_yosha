using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DormInfrastructure.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly DbDormContext _context;

        public StudentController(DbDormContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var students = _context.Students
                .Include(s => s.Fa)
                .Include(s => s.Ka);
            return View(await students.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Fa)
                .Include(s => s.Ka)
                .Include(s => s.Accommodations)
                    .ThenInclude(a => a.Ki)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName");
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName");
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FaId,KaId,StPib,StKurs,StDataNarodz,StTelefon")] Student student)
        {
            // ВИПРАВЛЕННЯ: Ігноруємо навігаційні властивості під час валідації форми
            ModelState.Remove("Fa");
            ModelState.Remove("Ka");

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName", student.FaId);
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName", student.KaId);
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName", student.FaId);
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName", student.KaId);
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FaId,KaId,StPib,StKurs,StDataNarodz,StTelefon")] Student student)
        {
            if (id != student.Id) return NotFound();

            // ВИПРАВЛЕННЯ: Ігноруємо навігаційні властивості під час редагування
            ModelState.Remove("Fa");
            ModelState.Remove("Ka");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FaId"] = new SelectList(_context.Faculties, "Id", "FaName", student.FaId);
            ViewData["KaId"] = new SelectList(_context.Departments, "Id", "KaName", student.KaId);
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Fa)
                .Include(s => s.Ka)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}