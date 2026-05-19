using DormDomain.Model;
using DormInfrastructure;
using DormInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Додали для роботи з Identity

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Налаштування підключення до бази даних PostgreSQL
builder.Services.AddDbContext<DbDormContext>(option =>
    option.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ==========================================================
// ДОДАЄМО СЕРВІСИ IDENTITY ДЛЯ КОРИСТУВАЧІВ ТА РОЛЕЙ
// ==========================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Спрощуємо вимоги до паролів для зручного тестування
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<DbDormContext>()
.AddDefaultTokenProviders();

// Налаштування кукі-файлів для збереження сесії входу
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";         // Шлях для входу
    options.AccessDeniedPath = "/Account/AccessDenied"; // Шлях при відсутності прав
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Обов'язковий порядок: спочатку ХТО ти (Authentication), потім ЩО тобі дозволено (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();