using System.ComponentModel.DataAnnotations;

namespace DormInfrastructure.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть повне ім'я")]
        [Display(Name = "Повне ім'я")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Пароль мінімум 4 символи")]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження пароля")]
        public string ConfirmPassword { get; set; } = null!;
    }
}