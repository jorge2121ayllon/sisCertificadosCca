using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.ViewModels;

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
        [Authorize]
        public ActionResult Index(string buscar, int pagina = 1)
        {
            //registrar el numero de registros por pagina
            int registrosPorPagina = 10;
           
            // CONSULTA BASE
            var query = _context.Alumnos
                .Where(a => a.Activo == true);


            // BUSCADOR
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(a =>
                    a.Nombres.Contains(buscar) ||
                    (a.Nombres + " " + a.Apellidos).Contains(buscar) ||
                    (a.Apellidos + " " + a.Nombres).Contains(buscar) ||
                    a.Apellidos.Contains(buscar) ||
                    a.Ci.Contains(buscar));
            }

            // TOTAL DE REGISTROS
            int totalRegistros = query.Count();

            // TOTAL PAGINAS
            int totalPaginas = (int)Math.Ceiling(
                (double)totalRegistros / registrosPorPagina
            );

            // PAGINACION
            var alumnos = query
                .OrderBy(a => a.Id)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            // ENVIAR A LA VISTA
            var vm = new AlumnoVM
            {
                Alumnos = alumnos,
                Buscar = buscar,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return View(vm);
          
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Alumno alumno)
        {
            try
            {
                alumno.FechaRegistro = DateTime.Now;

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Existen datos inválidos en el formulario.";
                    return View(alumno);
                }

                bool existeCi = _context.Alumnos.Any(a =>
                    a.Ci.ToUpper() == alumno.Ci.ToUpper());

                if (existeCi)
                {
                    TempData["Error"] = "El número de CI ya se encuentra registrado.";

                    ModelState.AddModelError(
                        nameof(alumno.Ci),
                        "El número de CI ya se encuentra registrado.");

                    return View(alumno);
                }

                alumno.Activo = true;

                _context.Alumnos.Add(alumno);
                _context.SaveChanges();

                TempData["Success"] = "Alumno registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(alumno);
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


        public IActionResult Buscar()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buscar(string ci)
        {

            if (ci == null) {
                TempData["Error"] = "Debe ingresar un CI para buscar al alumno.";
                return View();
            }

            var alumno = _context.Alumnos
                .Include(a => a.Inscripciones).
                    ThenInclude(i => i.Curso).
                Include(a => a.Inscripciones).
                    ThenInclude(i => i.Certificaciones)
                .FirstOrDefault(a => a.Ci == ci);

            if (alumno == null)
            {
                TempData["Error"] = "No se encontró ningún alumno con el CI proporcionado.";
                return View();
            }

              
            return View(alumno);  
            
           
                ;
          
        }

        
        // VER CERTIFICADO
        [HttpGet]
        public IActionResult VerCertificado(int inscripcionId)
        {
            var inscripcion = _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.Curso)
                .Include(i => i.Certificaciones)
                .FirstOrDefault(i => i.Id == inscripcionId);

            if (inscripcion == null)
            {
                TempData["Error"] = "No se encontró la inscripción.";
                return RedirectToAction("Buscar");
            }

            return View(inscripcion);
        }





    }
}
