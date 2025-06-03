using EVA_2.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EVA_2.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<ReportesPorPeriodo> VistaReporteCitas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Vista ReportesPorPeriodo
            modelBuilder.Entity<ReportesPorPeriodo>()
                .HasNoKey()
                .ToView("Vista_ReporteCitas"); // Asegúrate de que el nombre coincide con la vista creada en SQLite
            // Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired();
                entity.Property(e => e.Apellido).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Telefono).IsRequired();
                entity.Property(e => e.FechaRegistro).IsRequired();
            });

            // Servicio
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired();
                entity.Property(e => e.Descripcion).IsRequired();
                entity.Property(e => e.Duracion).IsRequired();
                entity.Property(e => e.Precio).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
            });

            // Cita
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Hora).IsRequired();
                entity.Property(e => e.FechaCambioEstado).IsRequired(false);

                entity.HasOne(e => e.Cliente)
                      .WithMany()
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Servicio)
                      .WithMany()
                      .HasForeignKey(e => e.ServicioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Relación con Estado


        }
    }
}