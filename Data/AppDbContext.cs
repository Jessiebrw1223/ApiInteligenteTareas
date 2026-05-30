using ApiInteligenteTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiInteligenteTareas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarea> Tareas => Set<Tarea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.ToTable("Tareas");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(120);
            entity.Property(t => t.Descripcion).HasMaxLength(500);
            entity.Property(t => t.Estado).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Prioridad).IsRequired().HasMaxLength(20);
            entity.Property(t => t.FechaCreacion).IsRequired();
            entity.Property(t => t.FechaVencimiento).IsRequired();
        });
    }
}
