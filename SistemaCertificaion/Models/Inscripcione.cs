using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaCertificaion.Models;

public partial class Inscripcione
{
    [Key]
    public int Id { get; set; }

    public int AlumnoId { get; set; }

    public int CursoId { get; set; }

    public bool Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaRegistro { get; set; }

    [ForeignKey("AlumnoId")]
    [InverseProperty("Inscripciones")]
    public virtual Alumno Alumno { get; set; } = null!;

    [InverseProperty("Inscripcion")]
    public virtual ICollection<Certificacione> Certificaciones { get; set; } = new List<Certificacione>();

    [ForeignKey("CursoId")]
    [InverseProperty("Inscripciones")]
    public virtual Curso Curso { get; set; } = null!;
}
