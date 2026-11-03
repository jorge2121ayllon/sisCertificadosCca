using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaCertificaion.Models;

namespace SistemaCertificaion.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Certificacione> Certificaciones { get; set; }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Alumnos__3214EC077B2F90A4");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Certificacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Certific__3214EC07A7D4B74E");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Inscripcion).WithMany(p => p.Certificaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__Inscr__5441852A");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cursos__3214EC073B14FBA0");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inscripc__3214EC074BAE7C77");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Alumno).WithMany(p => p.Inscripciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripci__Alumn__46E78A0C");

            entity.HasOne(d => d.Curso).WithMany(p => p.Inscripciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripci__Curso__47DBAE45");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC072C1E8792");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Rol).HasDefaultValue("ADMIN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
