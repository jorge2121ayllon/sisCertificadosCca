using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;

namespace SistemaCertificaion.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly AppDbContext _context ;
        public AlumnoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AlumnoController
        public ActionResult Index()
        {

            var alumnos = _context.Alumnos.Where(s=> s.Activo == true);         

            return View(alumnos);
        }

        // GET: AlumnoController/Details/5
        public ActionResult Details(int id)
        {
            var alumno = _context.Alumnos.FirstOrDefault(e => e.Id == id);
            return View(alumno);
        }

        // GET: AlumnoController/Create
        public ActionResult Create()
        {            
            return View();
        }

        // POST: AlumnoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Alumno alumno)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    alumno.Activo = true;
                    _context.Alumnos.Add(alumno);
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

        // GET: AlumnoController/Edit/5
        public ActionResult Edit(int id)
        {
            var alumno = _context.Alumnos.FirstOrDefault(a => a.Id == id);
            return View(alumno);
        }

        // POST: AlumnoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Alumno alumnoNuevo)
        {
            try
            {
                var AlumnoAntiguo = _context.Alumnos.FirstOrDefault(x => x.Id == id);
                AlumnoAntiguo.Nombres =alumnoNuevo.Nombres;
                AlumnoAntiguo.Apellidos=alumnoNuevo.Apellidos;
                AlumnoAntiguo.Ci=alumnoNuevo.Ci;
                AlumnoAntiguo.Celular =alumnoNuevo.Celular;
                AlumnoAntiguo.Email=alumnoNuevo.Email;

                _context.Alumnos.Update(AlumnoAntiguo);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AlumnoController/Delete/5
        public ActionResult Delete(int id)
        {
            var alumno = _context.Alumnos.FirstOrDefault(l => l.Id == id);
            return View(alumno);
        }

        // POST: AlumnoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Alumno alumno)
        {
            try
            {                
                var alumnos = _context.Alumnos.FirstOrDefault(k=>k.Id ==alumno.Id);
                alumnos.Activo = alumno.Activo;
                
                _context.Alumnos.Update(alumnos);
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
