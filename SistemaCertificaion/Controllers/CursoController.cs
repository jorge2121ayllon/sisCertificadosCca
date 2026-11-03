using Azure.Core;
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
    [Authorize]
    public class CursoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CursoController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: CursoController
        public ActionResult Index(string buscar, int pagina = 1)
        {
            int registrosPorPagina = 10;

            var query = _context.Cursos
                .Where(c => c.Activo == true);

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(buscar) ||
                    (c.Nombre + " " + c.Codigo).Contains(buscar) ||
                    (c.Codigo + " " + c.Nombre).Contains(buscar) ||
                    c.Codigo.Contains(buscar));
            }

            int totalRegistros = query.Count();

            int totalPaginas = (int)Math.Ceiling(
                (double)totalRegistros / registrosPorPagina
            );

            var cursos = query
                .OrderBy(c => c.Id)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            var vm = new CursoVM
            {
                Cursos = cursos,
                Buscar = buscar,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return View(vm);
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
                curso.FechaRegistro = DateTime.Now;

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Existen datos inválidos en el formulario.";
                    return View(curso);
                }

                bool existeCodigo = _context.Cursos.Any(c =>
                    c.Codigo.ToUpper() == curso.Codigo.ToUpper());

                if (existeCodigo)
                {
                    TempData["Error"] = "El código del curso ya se encuentra registrado.";

                    ModelState.AddModelError(
                        nameof(curso.Codigo),
                        "El código del curso ya se encuentra registrado.");

                    return View(curso);
                }

                curso.Activo = true;

                _context.Cursos.Add(curso);
                _context.SaveChanges();

                TempData["Success"] = "Curso registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(curso);
            }
        }

        // GET: CursoController/Edit/5
        public ActionResult Edit(int id)
        {
            var curso = _context.Cursos.FirstOrDefault(c => c.Id == id);
            return View(curso);
        }

        // POST: CursoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Curso cursoNuevo)
        {
            try
            {
                var CursoAntiguo = _context.Cursos
                    .FirstOrDefault(a => a.Id == id);

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
            var curso = _context.Cursos.FirstOrDefault(c => c.Id == id);
            return View(curso);
        }

        // POST: CursoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Curso curso)
        {
            try
            {
                var cursos = _context.Cursos
                    .FirstOrDefault(k => k.Id == curso.Id);

                curso.Activo = false;

                _context.Cursos.Update(cursos);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CursoController/Inscripcion
        public ActionResult Inscripcion(int id)
        {
            var curso = _context.Cursos
                .FirstOrDefault(c => c.Id == id && c.Activo == true);

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

        // POST: AdicionarAlumno
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

        // POST: QuitarAlumno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuitarAlumno(int inscripcionId, int cursoId)
        {
            var insc = _context.Inscripciones
                .FirstOrDefault(i => i.Id == inscripcionId);

            if (insc != null)
            {
                insc.Activo = false;

                _context.Inscripciones.Update(insc);
                _context.SaveChanges();
            }

            return RedirectToAction("Inscripcion", new { id = cursoId });
        }

        // GET: CursoController/Certificado
        public IActionResult Certificado(int id)
        {
            var curso = _context.Cursos
                .FirstOrDefault(c => c.Id == id && c.Activo == true);

            if (curso == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var inscripciones = _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.Certificaciones)
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

        // POST: AgregarCertificado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCertificado(int inscripcionId, int cursoId)
        {
            var existe = _context.Certificaciones
                .Any(c => c.InscripcionId == inscripcionId && c.Activo == true);

            if (existe)
            {
                TempData["Error"] = "Este alumno ya tiene certificado registrado.";

                return RedirectToAction(nameof(Certificado), new { id = cursoId });
            }

            var certificado = new Certificacione
            {
                InscripcionId = inscripcionId,

                Ispi = false,
                SmsRetiro = false,
                Proceso = false,
                Generado = false,
                Impresion = false,
                EnviadoDna = false,
                Finalizado = false,
                Entregado = false,

                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Certificaciones.Add(certificado);
            _context.SaveChanges();

            TempData["Mensaje"] = "Certificado agregado correctamente.";

            return RedirectToAction(nameof(Certificado), new { id = cursoId });
        }

        // POST: ActualizarCertificado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCertificado(
            int certificadoId,
            int cursoId,
            bool ispi,
            bool smsRetiro,
            bool proceso,
            bool generado,
            bool impresion,
            bool enviadoDna,
            bool finalizado,
            bool entregado,
            string? observacion,
            IFormFile? archivo)
        {
            var certificado = _context.Certificaciones
                .FirstOrDefault(c => c.Id == certificadoId && c.Activo == true);

            if (certificado == null)
            {
                TempData["Error"] = "No se encontró el certificado.";

                return RedirectToAction(nameof(Certificado), new { id = cursoId });
            }

            // ==============================
            // SUBIR ARCHIVO
            // ==============================

            if (archivo != null && archivo.Length > 0)
            {
                // Validar tamaño máximo: 10 MB
                if (archivo.Length > 10 * 1024 * 1024)
                {
                    TempData["Error"] = "El archivo no debe superar los 10 MB.";

                    return RedirectToAction(nameof(Certificado), new { id = cursoId });
                }

                // Extensiones permitidas
                var extensionesPermitidas = new[]
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx"
                };

                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    TempData["Error"] = "Formato de archivo no permitido. Solo PDF, Word y Excel.";

                    return RedirectToAction(nameof(Certificado), new { id = cursoId });
                }

                // Crear carpeta si no existe
                var carpeta = Path.Combine(
                    _environment.WebRootPath,
                    "archivos",
                    "certificados"
                );

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // Nombre único para el archivo
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";

                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                // Guardar archivo en el servidor
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

                // Guardar ruta en UrlArchivo
                certificado.UrlArchivo = $"/archivos/certificados/{nombreArchivo}";
            }

            // ==============================
            // ACTUALIZAR ESTADOS
            // ==============================

            certificado.Ispi = ispi;
            certificado.SmsRetiro = smsRetiro;
            certificado.Proceso = proceso;
            certificado.Generado = generado;
            certificado.Impresion = impresion;
            certificado.EnviadoDna = enviadoDna;
            certificado.Finalizado = finalizado;
            certificado.Entregado = entregado;

            // ==============================
            // ACTUALIZAR OBSERVACIÓN
            // ==============================

            certificado.Observacion = observacion;

            // ==============================
            // ACTUALIZAR FECHAS
            // ==============================

            if (ispi && certificado.FechaIspi == null)
                certificado.FechaIspi = DateTime.Now;

            if (!ispi)
                certificado.FechaIspi = null;

            if (smsRetiro && certificado.FechaSmsRetiro == null)
                certificado.FechaSmsRetiro = DateOnly.FromDateTime(DateTime.Now);

            if (!smsRetiro)
                certificado.FechaSmsRetiro = null;

            if (proceso && certificado.FechaProceso == null)
                certificado.FechaProceso = DateOnly.FromDateTime(DateTime.Now);

            if (!proceso)
                certificado.FechaProceso = null;

            if (generado && certificado.FechaGenerado == null)
                certificado.FechaGenerado = DateOnly.FromDateTime(DateTime.Now);

            if (!generado)
                certificado.FechaGenerado = null;

            if (impresion && certificado.FechaImpresion == null)
                certificado.FechaImpresion = DateOnly.FromDateTime(DateTime.Now);

            if (!impresion)
                certificado.FechaImpresion = null;

            if (enviadoDna && certificado.FechaEnviadoDna == null)
                certificado.FechaEnviadoDna = DateOnly.FromDateTime(DateTime.Now);

            if (!enviadoDna)
                certificado.FechaEnviadoDna = null;

            if (finalizado && certificado.FechaFinalizado == null)
                certificado.FechaFinalizado = DateOnly.FromDateTime(DateTime.Now);

            if (!finalizado)
                certificado.FechaFinalizado = null;

            if (entregado && certificado.FechaEntregado == null)
                certificado.FechaEntregado = DateOnly.FromDateTime(DateTime.Now);

            if (!entregado)
                certificado.FechaEntregado = null;

            // ==============================
            // GUARDAR CAMBIOS
            // ==============================

            _context.Certificaciones.Update(certificado);
            _context.SaveChanges();

            TempData["Mensaje"] = "Certificado actualizado correctamente.";

            return RedirectToAction(nameof(Certificado), new { id = cursoId });
        }

        // CAMBIAR ESTADO: Pendiente <-> Tiqueado
        [HttpGet]
        public IActionResult CambiarEstado(int certificadoId, string accion)
        {
            var certificado = _context.Certificaciones
                .Include(c => c.Inscripcion)
                .FirstOrDefault(c => c.Id == certificadoId && c.Activo == true);

            if (certificado == null)
            {
                TempData["Error"] = "No se encontró el certificado.";

                return RedirectToAction(nameof(Index));
            }

            // ISPI
            if (accion == "Ispi")
            {
                certificado.Ispi = !certificado.Ispi;

                certificado.FechaIspi = certificado.Ispi
                    ? DateTime.Now
                    : null;
            }

            // SMS RETIRO
            else if (accion == "SmsRetiro")
            {
                certificado.SmsRetiro = !certificado.SmsRetiro;

                certificado.FechaSmsRetiro = certificado.SmsRetiro
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // PROCESO
            else if (accion == "Proceso")
            {
                certificado.Proceso = !certificado.Proceso;

                certificado.FechaProceso = certificado.Proceso
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // GENERADO
            else if (accion == "Generado")
            {
                certificado.Generado = !certificado.Generado;

                certificado.FechaGenerado = certificado.Generado
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // IMPRESIÓN
            else if (accion == "Impresion")
            {
                certificado.Impresion = !certificado.Impresion;

                certificado.FechaImpresion = certificado.Impresion
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // ENVIADO DNA
            else if (accion == "EnviadoDna")
            {
                certificado.EnviadoDna = !certificado.EnviadoDna;

                certificado.FechaEnviadoDna = certificado.EnviadoDna
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // FINALIZADO
            else if (accion == "Finalizado")
            {
                certificado.Finalizado = !certificado.Finalizado;

                certificado.FechaFinalizado = certificado.Finalizado
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            // ENTREGADO
            else if (accion == "Entregado")
            {
                certificado.Entregado = !certificado.Entregado;

                certificado.FechaEntregado = certificado.Entregado
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;
            }

            else
            {
                TempData["Error"] = "Acción no reconocida.";

                return Redirect(Request.Headers["Referer"].ToString());
            }

            _context.Certificaciones.Update(certificado);
            _context.SaveChanges();

            TempData["Mensaje"] = "Estado del certificado actualizado correctamente.";

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}