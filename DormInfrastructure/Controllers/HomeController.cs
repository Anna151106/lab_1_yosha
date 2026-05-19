using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DormInfrastructure.Models;
using ClosedXML.Excel;
using DormDomain.Model;

namespace DormInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbDormContext _context;

        public HomeController(DbDormContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Charts
        public async Task<IActionResult> Charts()
        {
            var studentsByFaculty = await _context.Faculties
                .Select(f => new { Name = f.FaName, Count = f.Students.Count })
                .ToListAsync();

            var roomsByDorm = await _context.Dorms
                .Select(d => new { Name = "Гурт. №" + d.GuNomer, Count = d.Rooms.Count })
                .ToListAsync();

            var accommodationsByDorm = await _context.Dorms
                .Select(d => new
                {
                    Name = "Гурт. №" + d.GuNomer,
                    Count = d.Rooms.SelectMany(r => r.Accommodations)
                                   .Count(a => a.PrDataVysel == null)
                }).ToListAsync();

            ViewBag.FacultyLabels = studentsByFaculty.Select(x => x.Name).ToList();
            ViewBag.FacultyData = studentsByFaculty.Select(x => x.Count).ToList();
            ViewBag.DormLabels = roomsByDorm.Select(x => x.Name).ToList();
            ViewBag.RoomData = roomsByDorm.Select(x => x.Count).ToList();
            ViewBag.AccommodationData = accommodationsByDorm.Select(x => x.Count).ToList();

            return View();
        }

        // ============================================================
        // ЕКСПОРТ
        // ============================================================

        public async Task<IActionResult> ExportFacultyToExcel()
        {
            var data = await _context.Faculties
                .Select(f => new { Name = f.FaName, Count = f.Students.Count })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Факультети");

            ws.Cell(1, 1).Value = "Назва факультету";
            ws.Cell(1, 2).Value = "Кількість студентів";
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;

            int row = 2;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.Name ?? "Не вказано";
                ws.Cell(row, 2).Value = item.Count;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Zvit_Faculties.xlsx");
        }

        public async Task<IActionResult> ExportDormsToExcel()
        {
            var data = await _context.Dorms
                .Select(d => new
                {
                    Nomer = d.GuNomer,
                    TotalRooms = d.Rooms.Count,
                    CurrentResidents = d.Rooms.SelectMany(r => r.Accommodations)
                                              .Count(a => a.PrDataVysel == null)
                }).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Гуртожитки");

            ws.Cell(1, 1).Value = "Номер гуртожитку";
            ws.Cell(1, 2).Value = "Кількість кімнат";
            ws.Cell(1, 3).Value = "Поточних мешканців";
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;

            int row = 2;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = "Гуртожиток №" + item.Nomer;
                ws.Cell(row, 2).Value = item.TotalRooms;
                ws.Cell(row, 3).Value = item.CurrentResidents;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Zvit_Dorms.xlsx");
        }

        // ============================================================
        // ІМПОРТ
        // ============================================================

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Файл не вибрано або він порожній.";
                return View("Import");
            }

            var imported = 0;
            var errors = new List<string>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1);
            var rows = ws.RangeUsed().RowsUsed().Skip(1);

            foreach (var excelRow in rows)
            {
                try
                {
                    var pib = excelRow.Cell(1).GetString();
                    var kursStr = excelRow.Cell(2).GetString();
                    var telefon = excelRow.Cell(3).GetString();
                    var faName = excelRow.Cell(4).GetString();
                    var kaName = excelRow.Cell(5).GetString();

                    if (string.IsNullOrWhiteSpace(pib)) continue;

                    var faculty = await _context.Faculties
                        .FirstOrDefaultAsync(f => f.FaName == faName);
                    var department = await _context.Departments
                        .FirstOrDefaultAsync(d => d.KaName == kaName);

                    if (faculty == null)
                    {
                        errors.Add($"Рядок {excelRow.RowNumber()}: факультет '{faName}' не знайдено.");
                        continue;
                    }
                    if (department == null)
                    {
                        errors.Add($"Рядок {excelRow.RowNumber()}: кафедра '{kaName}' не знайдено.");
                        continue;
                    }

                    var student = new Student
                    {
                        StPib = pib,
                        StKurs = int.TryParse(kursStr, out var k) ? k : null,
                        StTelefon = telefon,
                        FaId = faculty.Id,
                        KaId = department.Id
                    };

                    _context.Students.Add(student);
                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Рядок {excelRow.RowNumber()}: помилка — {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            ViewBag.Success = $"Успішно імпортовано: {imported} студентів.";
            ViewBag.Errors = errors;
            return View("Import");
        }

        public IActionResult DownloadTemplate()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Студенти");

            ws.Cell(1, 1).Value = "ПІБ";
            ws.Cell(1, 2).Value = "Курс";
            ws.Cell(1, 3).Value = "Телефон";
            ws.Cell(1, 4).Value = "Назва факультету";
            ws.Cell(1, 5).Value = "Назва кафедри";
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Template_Students.xlsx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}