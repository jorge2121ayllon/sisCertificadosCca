
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaCertificaion.Data;
using SistemaCertificaion.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PasswordService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ruta del login
        options.LoginPath = "/Auth/Login";

        // Acceso denegado
        options.AccessDeniedPath = "/Auth/Denied";

        // Nombre de la cookie
        options.Cookie.Name = "SistemaCertificacion";

        // Tiempo de expiración
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        // Renovar automáticamente
        options.SlidingExpiration = true;

        // Seguridad
        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// Construir aplicación
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
