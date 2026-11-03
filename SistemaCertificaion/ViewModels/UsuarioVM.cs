using SistemaCertificaion.Models;

namespace SistemaCertificaion.ViewModels
{
    public class UsuarioVM
    {
        // LISTA DE USUARIOS
        public List<Usuario> Usuarios  { get; set; } = new();

        // TEXTO DEL BUSCADOR
        public string Buscar { get; set; }

        // PAGINA ACTUAL
        public int PaginaActual { get; set; }

        // TOTAL DE PAGINAS
        public int TotalPaginas { get; set; }

    }
}
