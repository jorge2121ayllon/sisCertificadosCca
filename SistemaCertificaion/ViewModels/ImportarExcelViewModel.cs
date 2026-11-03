using System.ComponentModel.DataAnnotations;

namespace SistemaCertificaion.ViewModels
{
    public class ImportarExcelViewModel
    {
        [Required(ErrorMessage = "Seleccione un archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

    }
}
