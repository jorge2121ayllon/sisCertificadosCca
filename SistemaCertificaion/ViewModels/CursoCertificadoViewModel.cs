using SistemaCertificaion.Models;

namespace SistemaCertificaion.ViewModels
{
    public class CursoCertificadoViewModel
    {
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public List<Inscripcione> Inscripciones { get; set; }
    }
}
