using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.Property(g => g.ClienteId).IsRequired();
        builder.Property(g => g.EmpleadoId).IsRequired(false);
        builder.Property(g => g.FechaHoraVenta)
            .IsRequired()
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
        builder.Property(g => g.NumeroOperacion)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired();
        builder.Property(g => g.MontoTotalVenta)
            .IsRequired()
            .HasColumnType("decimal(10,2)");
        builder.Property(g => g.CantidadTotalArticulos).IsRequired();
        builder.ToTable("Venta","Ventas");
    }
}