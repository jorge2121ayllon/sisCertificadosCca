using SistemaCertificaion.Models;

namespace SistemaCertificaion.ViewModels
{
    public class AlumnoVM
    {
        // LISTA DE ALUMNOS
        public List<Alumno> Alumnos { get; set; } = new();

        // TEXTO DEL BUSCADOR
        public string Buscar { get; set; }

        // PAGINA ACTUAL
        public int PaginaActual { get; set; }

        // TOTAL DE PAGINAS
        public int TotalPaginas { get; set; }

    }
}
