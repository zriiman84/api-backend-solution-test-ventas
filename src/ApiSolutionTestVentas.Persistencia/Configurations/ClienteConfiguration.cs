using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiSolutionTestVentas.Persistencia.Configurations;

public class ClienteConfiguration :IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(g => g.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(g => g.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(g => g.Email).HasMaxLength(100)
            .IsRequired()
            .IsUnicode(false);
        builder.HasIndex(p => p.Nombre);
        builder.HasIndex(p => p.Email).IsUnique();
        builder.ToTable("Cliente","Ventas");
        //Excluye los inactivos
        //builder.HasQueryFilter(x => x.Status);
        
    }
}
