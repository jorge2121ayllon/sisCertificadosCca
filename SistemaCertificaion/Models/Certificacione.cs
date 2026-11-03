    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    namespace SistemaCertificaion.Models;

    public partial class Certificacione
    {
        [Key]
        public int Id { get; set; }

        public int InscripcionId { get; set; }

        public bool Ispi { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FechaIspi { get; set; }

        [Unicode(false)]
        public string? Observacion { get; set; }
        public string? UrlArchivo { get; set; }

        public bool SmsRetiro { get; set; }

        public DateOnly? FechaSmsRetiro { get; set; }

        public bool Proceso { get; set; }

        public DateOnly? FechaProceso { get; set; }

        public bool Generado { get; set; }

        public DateOnly? FechaGenerado { get; set; }

        public bool Impresion { get; set; }

        public DateOnly? FechaImpresion { get; set; }

        public bool EnviadoDna { get; set; }

        public DateOnly? FechaEnviadoDna { get; set; }

        public bool Finalizado { get; set; }

        public DateOnly? FechaFinalizado { get; set; }

        public bool Entregado { get; set; }

        public DateOnly? FechaEntregado { get; set; }

        public bool Activo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime FechaRegistro { get; set; }

        [ForeignKey("InscripcionId")]
        [InverseProperty("Certificaciones")]
        public virtual Inscripcione Inscripcion { get; set; } = null!;
    }
