using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.Services;
using SistemaCertificaion.ViewModels;
using System.Security.Claims;

namespace SistemaCertificaion.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public AuthController(
            AppDbContext  context,
            PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas");

                return View(model);
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError("", "Usuario inactivo");

                return View(model);
            }

            bool passwordCorrecta =
                _passwordService.VerifyPassword(
                    model.Password,
                    usuario.Clave);

            if (!passwordCorrecta)
            {
                ModelState.AddModelError("", "Credenciales incorrectas");

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.Recordarme
                });

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}
