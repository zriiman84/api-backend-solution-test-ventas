using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class PuestoConfiguration : IEntityTypeConfiguration<Puesto>
{
    public void Configure(EntityTypeBuilder<Puesto> builder)
    {
        builder.Property(g => g.NombrePuesto).HasMaxLength(100).IsRequired();
        builder.Property(g => g.DescripcionPuesto).HasMaxLength(200).IsRequired(false);
        builder.Property(g => g.FechaCreacion)
            .IsRequired()
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
        builder.Property(g => g.FechaBaja)
            .IsRequired(false)
            .HasColumnType("datetime");
        builder.ToTable("Puesto","RRHH");
        builder.HasQueryFilter(x => x.Status); //Excluye los inactivos
    }
}