using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaCertificaion.Models;

public partial class Usuario
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NombreUsuario { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Clave { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Rol { get; set; } = null!;

    public bool Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaRegistro { get; set; }
}
