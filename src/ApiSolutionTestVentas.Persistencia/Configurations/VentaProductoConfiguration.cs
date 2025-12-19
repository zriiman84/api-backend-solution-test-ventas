using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class VentaProductoConfiguration : IEntityTypeConfiguration<VentaProducto>
{
    public void Configure(EntityTypeBuilder<VentaProducto> builder)
    {
        builder.Property(g => g.ProductoId).IsRequired();
        builder.Property(g => g.VentaId).IsRequired();
        builder.Property(g => g.PrecioCompra) //Es el precio con el que se compró el producto.
            .IsRequired()
            .HasColumnType("decimal(8,2)");
        builder.Property(g => g.SubTotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");
        builder.Property(g => g.Cantidad).IsRequired();
        builder.ToTable("VentaProducto","Ventas");
    }
}