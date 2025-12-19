using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApiSolutionTestVentas.Persistencia;

public class ApplicationDbContext : IdentityDbContext<SpecificUserIdentity>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    //Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Ignore<EmpleadoInfo>(); //No considerar esta entidad como tabla en la BD
        modelBuilder.Ignore<DetalleVenta>(); //No considerar esta entidad como tabla en la BD
        modelBuilder.Ignore<DetalleVentaInfo>(); //No considerar esta entidad como tabla en la BD
        modelBuilder.Ignore<VentaReporteClienteInfo>(); //No considerar esta entidad como tabla en la BD
        modelBuilder.Ignore<VentaReporteProductoInfo>(); //No considerar esta entidad como tabla en la BD

        //Configurar los las tablas User / Role / UserRole
        modelBuilder.Entity<SpecificUserIdentity>(x => x.ToTable("User"));
        modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role"));
        modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole"));
    }

    //Lazy Loading
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
