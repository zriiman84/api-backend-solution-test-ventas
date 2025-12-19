using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
{
    public void Configure(EntityTypeBuilder<CategoriaProducto> builder)
    {
        builder.Property(g => g.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Descripcion).HasMaxLength(250).IsRequired(false);
        builder.ToTable("CategoriaProducto", "Ventas");
        //Excluye los inactivos
        builder.HasQueryFilter(x => x.Status);

    }
}