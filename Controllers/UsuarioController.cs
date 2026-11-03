using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.Services;

namespace SistemaCertificaion.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        public UsuarioController(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            var usuario = _context.Usuarios.ToList();
            return View(usuario);
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
                if(ModelState.IsValid)
                {
                    usuario.Activo=true;
                    usuario.Clave = _passwordService.HashPassword(usuario.Clave);
                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch
            {
                return View();
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
                var usuarioAntiguo = _context.Usuarios.FirstOrDefault(f=>f.Id==id);

                usuarioAntiguo.NombreUsuario = UsuarioNuevo.NombreUsuario;
                usuarioAntiguo.Email = UsuarioNuevo.Email;
                usuarioAntiguo.Clave = UsuarioNuevo.Clave;
                usuarioAntiguo.Rol = UsuarioNuevo.Rol;

                _context.Update(usuarioAntiguo);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            var usuarios = _context.Usuarios.FirstOrDefault(k=>k.Id==id);
            return View(usuarios);
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                var usuarios =  _context.Usuarios.FirstOrDefault(p=>p.Id==id);
                usuario.Activo= false;

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
