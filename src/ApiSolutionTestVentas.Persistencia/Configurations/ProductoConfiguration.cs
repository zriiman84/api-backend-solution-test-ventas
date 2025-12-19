using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.Property(g => g.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Descripcion).HasMaxLength(200).IsRequired(false);
        builder.Property(g => g.DescripcionExtensa).HasMaxLength(800).IsRequired(false);
        builder.Property(g => g.PrecioUnitario)
            .IsRequired()
            .HasColumnType("decimal(8,2)");
        builder.Property(g => g.ImageUrl)
            .HasMaxLength(200)
            .IsUnicode(false)
            .IsRequired(false);
        builder.Property(g => g.Stock).IsRequired();
        builder.HasIndex(g => g.Nombre);
        builder.Property(g => g.CategoriaProductoId).IsRequired();
        builder.ToTable("Producto","Ventas");
        builder.HasQueryFilter(g => g.Status); //Excluye los inactivos
    }
}