using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DormInfrastructure.Models;
using ClosedXML.Excel;
using System.IO;

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
            // Студентів по факультетах
            var studentsByFaculty = await _context.Faculties
                .Select(f => new
                {
                    Name = f.FaName,
                    Count = f.Students.Count
                }).ToListAsync();

            // Кімнат по гуртожитках
            var roomsByDorm = await _context.Dorms
                .Select(d => new
                {
                    Name = "Гурт. №" + d.GuNomer,
                    Count = d.Rooms.Count
                }).ToListAsync();

            // Заселених студентів по гуртожитках
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

        // ==========================================================
        // МЕТОДИ ДЛЯ ЕКСПОРТУ В EXCEL (ЕТАП 1.6)
        // ==========================================================

        // 1. Метод експорту звіту по факультетах
        public async Task<IActionResult> ExportFacultyToExcel()
        {
            var data = await _context.Faculties
                .Select(f => new
                {
                    Name = f.FaName,
                    Count = f.Students.Count
                }).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Факультети");

                // Заголовки таблиці
                worksheet.Cell(1, 1).Value = "Назва факультету";
                worksheet.Cell(1, 2).Value = "Кількість студентів";

                // Робимо заголовок жирним
                worksheet.Row(1).Style.Font.Bold = true;

                // Заповнюємо рядки даними
                int currentRow = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(currentRow, 1).Value = item.Name ?? "Не вказано";
                    worksheet.Cell(currentRow, 2).Value = item.Count;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Zvit_Faculties.xlsx"
                    );
                }
            }
        }

        // 2. Метод експорту звіту по гуртожитках
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

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Гуртожитки");

                // Заголовки таблиці
                worksheet.Cell(1, 1).Value = "Номер гуртожитку";
                worksheet.Cell(1, 2).Value = "Загальна кількість кімнат";
                worksheet.Cell(1, 3).Value = "Кількість поточних мешканців";

                worksheet.Row(1).Style.Font.Bold = true;

                int currentRow = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(currentRow, 1).Value = "Гуртожиток №" + item.Nomer;
                    worksheet.Cell(currentRow, 2).Value = item.TotalRooms;
                    worksheet.Cell(currentRow, 3).Value = item.CurrentResidents;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Zvit_Dorms.xlsx"
                    );
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}