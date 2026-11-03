using SistemaCertificaion.Models;

namespace SistemaCertificaion.ViewModels
{
    public class GestionAcademicaViewModel
    {
        public int? CursoId { get; set; }

        public Alumno NuevoAlumno { get; set; } = new();

        public Curso NuevoCurso { get; set; } = new();

        public List<Curso> Cursos { get; set; } = new();

        public Curso? CursoSeleccionado { get; set; }

        public List<Alumno> Alumnos { get; set; } = new();

        public List<Inscripcione> Inscripciones { get; set; } = new();

        public int TotalInscritos { get; set; }

        public int TotalCertificados { get; set; }

        public int TotalEntregados { get; set; }

        public int TotalPendientes { get; set; }
    }
}
