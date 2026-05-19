using System.Collections.Generic;

namespace DormMVC.Models
{
    // Модель для першого графіка: Студенти по факультетах
    public class FacultyAnalyticsViewModel
    {
        public string FacultyName { get; set; }
        public int StudentCount { get; set; }
    }

    // Модель для другого графіка: Кімнати та мешканці по гуртожитках
    public class DormAnalyticsViewModel
    {
        public string DormName { get; set; }
        public int TotalRooms { get; set; }
        public int CurrentResidents { get; set; }
    }

    // Загальний контейнер, який ми передамо в сторінку
    public class AnalyticsPageViewModel
    {
        public List<FacultyAnalyticsViewModel> FacultyData { get; set; } = new List<FacultyAnalyticsViewModel>();
        public List<DormAnalyticsViewModel> DormData { get; set; } = new List<DormAnalyticsViewModel>();
    }
}