using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.Property(g => g.NombreDepartamento).HasMaxLength(100).IsRequired();
        builder.Property(g => g.FechaCreacion)
            .IsRequired()
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
        builder.Property(g => g.FechaBaja)
            .IsRequired(false)
            .HasColumnType("datetime");
        builder.ToTable("Departamento","RRHH");
        builder.HasQueryFilter(x => x.Status); //Excluye los inactivos
    }
}