using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaCertificaion.Models;

[Index("Codigo", Name = "UQ__Cursos__06370DAC96D651AC", IsUnique = true)]
public partial class Curso
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaRegistro { get; set; }

    [InverseProperty("Curso")]
    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
