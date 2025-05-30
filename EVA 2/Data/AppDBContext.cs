using EVA_2.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EVA_2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                entity.Property(e => e.Estado).IsRequired();

                entity.HasOne(e => e.Cliente)
                      .WithMany()
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Servicio)
                      .WithMany()
                      .HasForeignKey(e => e.ServicioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}