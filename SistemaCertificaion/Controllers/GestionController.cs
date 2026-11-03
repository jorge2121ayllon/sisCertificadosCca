
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.ViewModels;

namespace SistemaCertificaion.Controllers
{
    public class GestionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private const long TamañoMaximoArchivo = 10 * 1024 * 1024; // 10 MB

        private static readonly string[] ExtensionesPermitidas =
        {
            ".pdf",
            ".docx",
            ".xlsx",
            ".png",
            ".jpg",
            ".jpeg"
        };

        public GestionController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ==========================================
        // GESTIÓN ACADÉMICA
        // ==========================================

        [HttpGet]
        public IActionResult Index(int? cursoId)
        {
            var vm = new GestionAcademicaViewModel();

            vm.Cursos = _context.Cursos
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToList();

            vm.Alumnos = _context.Alumnos
                .Where(x => x.Activo)
                .OrderBy(x => x.Apellidos)
                .ToList();

            if (cursoId.HasValue)
            {
                vm.CursoId = cursoId;

                vm.CursoSeleccionado = _context.Cursos
                    .FirstOrDefault(x => x.Id == cursoId.Value);

                vm.Inscripciones = _context.Inscripciones
                    .Include(x => x.Alumno)
                    .Include(x => x.Certificaciones)
                    .Where(x =>
                        x.CursoId == cursoId.Value &&
                        x.Activo)
                    .OrderBy(x => x.Alumno.Apellidos)
                    .ThenBy(x => x.Alumno.Nombres)
                    .ToList();

                vm.TotalInscritos = vm.Inscripciones.Count;

                vm.TotalCertificados = vm.Inscripciones.Count(x =>
                    x.Certificaciones.Any(c => c.Activo));

                vm.TotalEntregados = vm.Inscripciones.Count(x =>
                    x.Certificaciones.Any(c =>
                        c.Activo && c.Entregado));

                vm.TotalPendientes =
                    vm.TotalInscritos - vm.TotalEntregados;
            }

            return View(vm);
        }

        // ==========================================
        // ACTUALIZAR OBSERVACIÓN Y ARCHIVO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarObservacion(
            int certificadoId,
            int cursoId,
            string? observacion,
            IFormFile? archivo)
        {
            var certificado = await _context.Certificaciones
                .FirstOrDefaultAsync(x =>
                    x.Id == certificadoId &&
                    x.Activo);

            if (certificado == null)
            {
                TempData["Error"] =
                    "No se encontró el certificado.";

                return RedirectToAction(
                    nameof(Index),
                    new { cursoId });
            }

            // Actualizar observación
            certificado.Observacion = observacion;

            // Si se seleccionó un archivo, guardarlo
            if (archivo != null && archivo.Length > 0)
            {
                var extension = Path.GetExtension(archivo.FileName)
                    .ToLowerInvariant();

                if (!ExtensionesPermitidas.Contains(extension))
                {
                    TempData["Error"] =
                        "Formato no permitido. " +
                        "Solo se aceptan PDF, DOCX, XLSX, PNG y JPG.";

                    return RedirectToAction(
                        nameof(Index),
                        new { cursoId });
                }

                if (archivo.Length > TamañoMaximoArchivo)
                {
                    TempData["Error"] =
                        "El archivo supera el tamaño máximo permitido de 10 MB.";

                    return RedirectToAction(
                        nameof(Index),
                        new { cursoId });
                }

                var carpeta = Path.Combine(
                    _environment.WebRootPath,
                    "archivos",
                    "certificados");

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // Nombre único para evitar sobrescribir archivos
                var nombreArchivo =
                    $"{Guid.NewGuid():N}{extension}";

                var rutaFisica = Path.Combine(
                    carpeta,
                    nombreArchivo);

                using (var stream = new FileStream(
                    rutaFisica,
                    FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Ruta que se guarda en la base de datos
                certificado.UrlArchivo =
                    $"/archivos/certificados/{nombreArchivo}";
            }

            _context.Certificaciones.Update(certificado);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Observación y archivo actualizados correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { cursoId });
        }

        // ==========================================
        // CREAR CURSO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCurso(Curso curso)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Verifique los datos del curso.";

                return RedirectToAction(nameof(Index));
            }

            bool existe = _context.Cursos.Any(x =>
                x.Codigo == curso.Codigo);

            if (existe)
            {
                TempData["Error"] =
                    "El código del curso ya existe.";

                return RedirectToAction(nameof(Index));
            }

            curso.Activo = true;
            curso.FechaRegistro = DateTime.Now;

            _context.Cursos.Add(curso);
            _context.SaveChanges();

            TempData["Mensaje"] =
                "Curso registrado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { cursoId = curso.Id });
        }

        // ==========================================
        // INSCRIBIR ALUMNO EXISTENTE
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InscribirAlumno(
            int cursoId,
            int alumnoId)
        {
            bool cursoExiste = _context.Cursos.Any(x =>
                x.Id == cursoId && x.Activo);

            bool alumnoExiste = _context.Alumnos.Any(x =>
                x.Id == alumnoId && x.Activo);

            if (!cursoExiste || !alumnoExiste)
            {
                TempData["Error"] =
                    "El curso o el alumno no existe.";

                return RedirectToAction(
                    nameof(Index),
                    new { cursoId });
            }

            bool existeInscripcion =
                _context.Inscripciones.Any(x =>
                    x.CursoId == cursoId &&
                    x.AlumnoId == alumnoId &&
                    x.Activo);

            if (existeInscripcion)
            {
                TempData["Error"] =
                    "El alumno ya está inscrito en este curso.";

                return RedirectToAction(
                    nameof(Index),
                    new { cursoId });
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

            TempData["Mensaje"] =
                "Alumno inscrito correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { cursoId });
        }

        // ==========================================
        // REGISTRAR ALUMNO E INSCRIBIR
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearAlumnoInscribir(
            Alumno alumno,
            int cursoId)
        {
            bool cursoExiste = _context.Cursos.Any(x =>
                x.Id == cursoId && x.Activo);

            if (!cursoExiste)
            {
                TempData["Error"] =
                    "No se encontró el curso seleccionado.";

                return RedirectToAction(nameof(Index));
            }

            alumno.Activo = true;
            alumno.FechaRegistro = DateTime.Now;

            _context.Alumnos.Add(alumno);
            _context.SaveChanges();

            var inscripcion = new Inscripcione
            {
                CursoId = cursoId,
                AlumnoId = alumno.Id,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Inscripciones.Add(inscripcion);
            _context.SaveChanges();

            TempData["Mensaje"] =
                "Alumno registrado e inscrito correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { cursoId });
        }

        // ==========================================
        // AGREGAR CERTIFICADO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCertificado(
            int inscripcionId,
            int cursoId)
        {
            bool inscripcionExiste =
                _context.Inscripciones.Any(x =>
                    x.Id == inscripcionId &&
                    x.CursoId == cursoId &&
                    x.Activo);

            if (!inscripcionExiste)
            {
                TempData["Error"] =
                    "No se encontró la inscripción.";

                return RedirectToAction(
                    nameof(Index),
                    new { cursoId });
            }

            bool existeCertificado =
                _context.Certificaciones.Any(x =>
                    x.InscripcionId == inscripcionId &&
                    x.Activo);

            if (existeCertificado)
            {
                TempData["Error"] =
                    "Ya existe un certificado activo para esta inscripción.";

                return RedirectToAction(
                    nameof(Index),
                    new { cursoId });
            }

            var certificado = new Certificacione
            {
                InscripcionId = inscripcionId,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Certificaciones.Add(certificado);
            _context.SaveChanges();

            TempData["Mensaje"] =
                "Certificado agregado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { cursoId });
        }

        // ==========================================
        // CAMBIAR ESTADO DEL CERTIFICADO
        // ==========================================

        [HttpGet]
        public IActionResult CambiarEstado(
            int certificadoId,
            string accion)
        {
            var certificado = _context.Certificaciones
                .Include(c => c.Inscripcion)
                .FirstOrDefault(c =>
                    c.Id == certificadoId &&
                    c.Activo);

            if (certificado == null)
            {
                TempData["Error"] =
                    "No se encontró el certificado.";

                return RedirectToAction(nameof(Index));
            }

            switch (accion)
            {
                case "Ispi":
                    certificado.Ispi = true;
                    certificado.FechaIspi = DateTime.Now;
                    break;

                case "SmsRetiro":
                    certificado.SmsRetiro = true;
                    certificado.FechaSmsRetiro =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Proceso":
                    certificado.Proceso = true;
                    certificado.FechaProceso =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Generado":
                    certificado.Generado = true;
                    certificado.FechaGenerado =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Impresion":
                    certificado.Impresion = true;
                    certificado.FechaImpresion =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "EnviadoDna":
                    certificado.EnviadoDna = true;
                    certificado.FechaEnviadoDna =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Finalizado":
                    certificado.Finalizado = true;
                    certificado.FechaFinalizado =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Entregado":
                    certificado.Entregado = true;
                    certificado.FechaEntregado =
                        DateOnly.FromDateTime(DateTime.Now);
                    break;

                default:
                    TempData["Error"] =
                        "Acción no reconocida.";

                    return RedirectToAction(
                        nameof(Index),
                        new
                        {
                            cursoId = certificado.Inscripcion.CursoId
                        });
            }

            _context.Certificaciones.Update(certificado);
            _context.SaveChanges();

            TempData["Mensaje"] =
                "Estado actualizado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new
                {
                    cursoId = certificado.Inscripcion.CursoId
                });
        }
    }
}
