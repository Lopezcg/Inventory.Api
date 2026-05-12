using Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Cantidad).HasColumnName("cantidad").IsRequired();
        });
    }
}
