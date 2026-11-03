using SistemaCertificaion.Models;

namespace SistemaCertificaion.ViewModels
{
    public class CursoInscripcionViewModel
    {
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; } = string.Empty;
        public string NombreCurso {  get; set; } = string.Empty;
        public int AlumnoId { get; set; }
        public List<Alumno> Alumnos { get; set; } = new List<Alumno>();
        public List<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
    }
}
