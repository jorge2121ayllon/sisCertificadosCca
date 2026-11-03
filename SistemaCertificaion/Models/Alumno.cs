using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaCertificaion.Models;

public partial class Alumno
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombres { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Apellidos { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string? Ci { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Celular { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? Email { get; set; }

    public bool Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaRegistro { get; set; }

    [InverseProperty("Alumno")]
    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
