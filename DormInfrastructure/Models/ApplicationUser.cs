using Microsoft.AspNetCore.Identity;

namespace DormInfrastructure.Models
{
    // Цей клас розширює стандартного користувача Identity
    public class ApplicationUser : IdentityUser
    {
        // Якщо це студент, тут буде зберігатися його ST_ID з таблиці студентів
        // Щоб ми знали, який саме студент увійшов у систему
        public int? StudentId { get; set; }

        public string FullName { get; set; }
    }
}