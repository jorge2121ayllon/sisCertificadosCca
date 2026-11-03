using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using Microsoft.EntityFrameworkCore;
using SistemaCertificaion.ViewModels;


namespace SistemaCertificaion.Controllers
{
    public class CursoController : Controller
    {
        private readonly AppDbContext _context;
        public CursoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CursoController
        public ActionResult Index()
        {
            var cursos = _context.Cursos.Where(a=>a.Activo == true);
            return View(cursos);
        }

        // GET: CursoController/Details/5
        public ActionResult Details(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(a=>a.Id == id);
            return View(curso);
        }

        public ActionResult VerAlumnos(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(t => t.Id == id);
            return View(curso);
        }

        // GET: CursoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CursoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Curso curso)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    curso.Activo = true;
                    _context.Cursos.Add(curso);
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

        // GET: CursoController/Edit/5
        public ActionResult Edit(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(c=>c.Id ==id);
            return View(curso);
        }

        // POST: CursoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Curso cursoNuevo)
        {
            try
            {
                var CursoAntiguo = _context.Cursos.FirstOrDefault(a => a.Id == id);

                CursoAntiguo.Codigo = cursoNuevo.Codigo;
                CursoAntiguo.Nombre = cursoNuevo.Nombre;
                CursoAntiguo.FechaInicio = cursoNuevo.FechaInicio;
                CursoAntiguo.FechaFin = cursoNuevo.FechaFin;

                _context.Update(CursoAntiguo);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CursoController/Delete/5
        public ActionResult Delete(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(c=> c.Id == id);
            return View(curso);
        }

        // POST: CursoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Curso curso)
        {
            try
            {
                var cursos = _context.Cursos.FirstOrDefault(k=>k.Activo == true);
                curso.Activo = curso.Activo;

                _context.Cursos.Update(cursos);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        //Este si

        public ActionResult Inscripcion(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(c => c.Id == id && c.Activo == true);

            if (curso == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var alumnos = _context.Alumnos
                .Where(a => a.Activo == true)
                .ToList();

            var inscripciones = _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.Curso)
                .Where(i => i.CursoId == id && i.Activo == true)
                .ToList();

            var modelo = new CursoInscripcionViewModel
            {
                CursoId = curso.Id,
                CodigoCurso = curso.Codigo,
                NombreCurso = curso.Nombre,
                Alumnos = alumnos,
                Inscripciones = inscripciones
            };

            return View(modelo);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarAlumno(int cursoId, int alumnoId)
        {
            if (cursoId <= 0 || alumnoId <= 0)
            {
                return RedirectToAction(nameof(Inscripcion), new { id = cursoId });
            }

            var existe = _context.Inscripciones
                .Any(i => i.CursoId == cursoId
                       && i.AlumnoId == alumnoId
                       && i.Activo == true);

            if (existe)
            {
                TempData["Error"] = "El alumno ya está inscrito en este curso.";
                return RedirectToAction(nameof(Inscripcion), new { id = cursoId });
            }

            var inscripcion = new Inscripcione
            {
                CursoId = cursoId,
                AlumnoId = alumnoId,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Inscripciones.Add(inscripcion);
            _context.SaveChanges();

            TempData["Mensaje"] = "Alumno inscrito correctamente.";

            return RedirectToAction(nameof(Inscripcion), new { id = cursoId });
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarAlumno(int inscripcionId, int cursoId) 
        {
            var inscripcion = _context.Inscripciones.FirstOrDefault(g => g.Id == inscripcionId);

            inscripcion.Activo = false;

            _context.Inscripciones.Update(inscripcion);
            _context.SaveChanges();

            return RedirectToAction(nameof(Inscripcion), new { id = cursoId });
        }
        public IActionResult Certificado(int id)
        {
            //curso
            var curso = _context.Cursos.FirstOrDefault(c => c.Id == id && c.Activo == true);
            //inscripciones con el curso obtenido
            var inscripciones = _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.Curso)
            .Where(i => i.CursoId == curso.Id && i.Activo == true)
                .ToList();
        var modelo = new CursoCertificadoViewModel
        { 
            CursoId = curso.Id, 
            CodigoCurso = curso.Codigo,
            NombreCurso = curso.Nombre,
            Inscripciones = inscripciones
        };
        return View(modelo);
    }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCertyificado(int inscripcionId, int cursoId)
        {
            var existe = _context.Certificaciones
                .Any(c => c.InscripcionId == inscripcionId
                && c.Activo == true);
            if (existe)
            {
                TempData["Error"] = "Este alumno ya tiene certificado registrado.";
                return RedirectToAction(nameof(Certificado), new object { id = cursoId})
                    Ispi = false,
                    SmsRetiro = false;
                    Proceso = false;
                    Generado = false;

            }
        }
}
