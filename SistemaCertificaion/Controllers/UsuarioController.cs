
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.Services;
using SistemaCertificaion.ViewModels;

namespace SistemaCertificaion.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public UsuarioController(AppDbContext context, PasswordService passwordService)
        {
            _passwordService = passwordService;
            _context = context;
        }

        // GET: UsuarioController
        public ActionResult Index(string buscar, int pagina = 1)
        {
            int registrosPorPagina = 10;

            var query = _context.Usuarios
                .Where(u => u.Activo == true);

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(u =>
                    u.NombreUsuario.Contains(buscar) ||
                    u.Email.Contains(buscar));
            }

            int totalRegistros = query.Count();

            int totalPaginas = (int)Math.Ceiling(
                (double)totalRegistros / registrosPorPagina
            );

            var usuarios = query
                .OrderBy(u => u.Id)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            var vm = new UsuarioVM
            {
                Usuarios = usuarios,
                Buscar = buscar,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return View(vm);
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Id == id);
            return View(usuario);
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                usuario.FechaRegistro = DateTime.Now;

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Existen datos inválidos en el formulario.";
                    return View(usuario);
                }

                bool existeUsuario = _context.Usuarios.Any(u =>
                    u.NombreUsuario.ToUpper() == usuario.NombreUsuario.ToUpper());

                if (existeUsuario)
                {
                    TempData["Error"] = "El nombre de usuario ya se encuentra registrado.";
                    return View(usuario);
                }

                bool existeEmail = _context.Usuarios.Any(u =>
                    u.Email.ToUpper() == usuario.Email.ToUpper());

                if (existeEmail)
                {
                    TempData["Error"] = "El correo electrónico ya se encuentra registrado.";
                    return View(usuario);
                }

                usuario.Clave = _passwordService.HashPassword(usuario.Clave);
                usuario.Activo = true;
                usuario.FechaRegistro = DateTime.Now;

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                TempData["Success"] = "Usuario registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(usuario);
            }
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(y => y.Id == id);
            return View(usuario);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario UsuarioNuevo)
        {
            try
            {
                var usuarioAntiguo = _context.Usuarios
                    .FirstOrDefault(f => f.Id == id);

                if (usuarioAntiguo == null)
                {
                    return NotFound();
                }

                usuarioAntiguo.NombreUsuario = UsuarioNuevo.NombreUsuario;
                usuarioAntiguo.Email = UsuarioNuevo.Email;
                usuarioAntiguo.Rol = UsuarioNuevo.Rol;

                // CORRECCION: Encriptar la nueva contraseña con BCrypt
                if (!string.IsNullOrWhiteSpace(UsuarioNuevo.Clave))
                {
                    usuarioAntiguo.Clave =
                        _passwordService.HashPassword(UsuarioNuevo.Clave);
                }

                _context.Update(usuarioAntiguo);
                _context.SaveChanges();

                TempData["Success"] = "Usuario actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(UsuarioNuevo);
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            var usuarios = _context.Usuarios.FirstOrDefault(k => k.Id == id);
            return View(usuarios);
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                var usuarios = _context.Usuarios.FirstOrDefault(p => p.Id == id);

                if (usuarios == null)
                {
                    return NotFound();
                }

                usuarios.Activo = false;

                _context.Usuarios.Update(usuarios);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
