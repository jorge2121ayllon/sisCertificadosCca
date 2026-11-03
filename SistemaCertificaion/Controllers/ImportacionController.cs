using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using SistemaCertificaion.Data;
using SistemaCertificaion.Models;
using SistemaCertificaion.ViewModels;

namespace SistemaCertificaion.Controllers
{
    public class ImportacionController : Controller
    {
        private readonly AppDbContext _context;

        public ImportacionController(
            AppDbContext context)
        {
            _context = context;
        }

        //----------------------------------------------------
        // FORMULARIO
        //----------------------------------------------------

        public IActionResult Index()
        {
            return View(
                new ImportarExcelViewModel());
        }

        //----------------------------------------------------
        // IMPORTAR
        //----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Importar(
            ImportarExcelViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Index", vm);

            var resultado =
                new ResultadoImportacionViewModel();

            using var stream =
                vm.ArchivoExcel.OpenReadStream();

            using var workbook =
                new XLWorkbook(stream);

            var hoja =
                workbook.Worksheet(1);

            var filas =
                hoja.RowsUsed().Skip(1);

            foreach (var fila in filas)
            {
                try
                {
                    //------------------------------------------------
                    // LEER EXCEL
                    //------------------------------------------------

                    string nombreCompleto =
                        fila.Cell(2).GetString();

                    string ci =
                        fila.Cell(5).GetString();

                    string celular =
                        fila.Cell(6).GetString();

                    string codigoCurso =
                        fila.Cell(7).GetString();

                    string nombreCurso =
                        fila.Cell(8).GetString();

                    bool ispi =
                        ConvertirBool(
                            fila.Cell(9).GetString());

                    string observacion =
                        fila.Cell(10).GetString();

                    bool smsRetiro =
                        ConvertirBool(
                            fila.Cell(11).GetString());

                    bool proceso =
                        ConvertirBool(
                            fila.Cell(12).GetString());

                    bool generado =
                        ConvertirBool(
                            fila.Cell(13).GetString());

                    bool impresion =
                        ConvertirBool(
                            fila.Cell(14).GetString());

                    bool enviadoDna =
                        ConvertirBool(
                            fila.Cell(15).GetString());

                    bool finalizado =
                        ConvertirBool(
                            fila.Cell(16).GetString());

                    bool entregado =
                        ConvertirBool(
                            fila.Cell(17).GetString());

                    //------------------------------------------------
                    // VALIDAR
                    //------------------------------------------------

                    if (string.IsNullOrWhiteSpace(ci))
                    {
                        resultado.Errores++;
                        continue;
                    }

                    //------------------------------------------------
                    // SEPARAR NOMBRE
                    //------------------------------------------------

                    SepararNombre(
                        nombreCompleto,
                        out string nombres,
                        out string apellidos);

                    //------------------------------------------------
                    // ALUMNO
                    //------------------------------------------------

                    var alumno =
                        _context.Alumnos
                        .FirstOrDefault(x =>
                            x.Ci == ci);

                    if (alumno == null)
                    {
                        alumno = new Alumno
                        {
                            Nombres = nombres,
                            Apellidos = apellidos,
                            Ci = ci,
                            Celular = celular,
                            Activo = true,
                            FechaRegistro = DateTime.Now
                        };

                        _context.Alumnos.Add(alumno);

                        await _context.SaveChangesAsync();

                        resultado.AlumnosCreados++;
                    }

                    //------------------------------------------------
                    // CURSO
                    //------------------------------------------------

                    var curso =
                        _context.Cursos
                        .FirstOrDefault(x =>
                            x.Codigo == codigoCurso);

                    if (curso == null)
                    {
                        curso = new Curso
                        {
                            Codigo = codigoCurso,
                            Nombre = nombreCurso,
                            Activo = true,
                            FechaRegistro = DateTime.Now
                        };

                        _context.Cursos.Add(curso);

                        await _context.SaveChangesAsync();

                        resultado.CursosCreados++;
                    }

                    //------------------------------------------------
                    // INSCRIPCION
                    //------------------------------------------------

                    var inscripcion =
                        _context.Inscripciones
                        .FirstOrDefault(x =>
                            x.AlumnoId == alumno.Id &&
                            x.CursoId == curso.Id);

                    if (inscripcion == null)
                    {
                        inscripcion =
                            new Inscripcione
                            {
                                AlumnoId = alumno.Id,
                                CursoId = curso.Id,
                                Activo = true,
                                FechaRegistro =
                                    DateTime.Now
                            };

                        _context.Inscripciones.Add(
                            inscripcion);

                        await _context.SaveChangesAsync();

                        resultado
                            .InscripcionesCreadas++;
                    }

                    //------------------------------------------------
                    // CERTIFICACION
                    //------------------------------------------------

                    bool existeCertificacion =
                        _context.Certificaciones
                        .Any(x =>
                            x.InscripcionId ==
                            inscripcion.Id);

                    if (existeCertificacion)
                    {
                        resultado.RegistrosOmitidos++;
                        continue;
                    }

                    var certificacion =
                        new Certificacione
                        {
                            InscripcionId =
                                inscripcion.Id,

                            Ispi = ispi,

                            Observacion =
                                observacion,

                            SmsRetiro =
                                smsRetiro,

                            Proceso =
                                proceso,

                            Generado =
                                generado,

                            Impresion =
                                impresion,

                            EnviadoDna =
                                enviadoDna,

                            Finalizado =
                                finalizado,

                            Entregado =
                                entregado,

                            Activo = true,

                            FechaRegistro =
                                DateTime.Now
                        };

                    _context.Certificaciones
                        .Add(certificacion);

                    resultado
                        .CertificacionesCreadas++;
                }
                catch
                {
                    resultado.Errores++;
                }
            }

            await _context.SaveChangesAsync();

            return View(
                "Resultado",
                resultado);
        }

        //----------------------------------------------------
        // AUXILIARES
        //----------------------------------------------------

        private bool ConvertirBool(
            string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            valor = valor
                .Trim()
                .ToUpper();

            return valor == "TRUE"
                || valor == "SI"
                || valor == "1";
        }

        private void SepararNombre(
            string nombreCompleto,
            out string nombres,
            out string apellidos)
        {
            var partes =
                nombreCompleto.Split(
                    ' ',
                    StringSplitOptions
                    .RemoveEmptyEntries);

            if (partes.Length >= 4)
            {
                nombres =
                    partes[0] + " " +
                    partes[1];

                apellidos =
                    partes[2] + " " +
                    partes[3];
            }
            else
            {
                nombres =
                    nombreCompleto;

                apellidos = "";
            }
        }
    }
}
