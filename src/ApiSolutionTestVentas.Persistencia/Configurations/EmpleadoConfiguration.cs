using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.Property(g => g.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(g => g.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(g => g.NroDocumento).HasMaxLength(20).IsRequired();
        builder.Property(g => g.Email).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Salario).IsRequired();
        builder.Property(g => g.JefeId).IsRequired(false);
        builder.Property(g => g.PuestoId).IsRequired();
        builder.Property(g => g.DepartamentoId).IsRequired();
        builder.Property(g => g.FechaIngreso)
            .IsRequired()
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
        builder.Property(g => g.FechaBaja)
            .IsRequired(false)
            .HasColumnType("datetime");
        builder.HasIndex(g => g.NroDocumento).IsUnique();       //índice - Unique
        builder.HasIndex(g => g.Nombre);    //índice
        builder.ToTable("Empleado","RRHH");
        //Excluye los inactivos
        //builder.HasQueryFilter(x => x.Status);
    }
}