using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class PuestoSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public PuestoSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        // Definir los puesto de trabajo que deseas añadir
        var listaPuestos = new List<Puesto>();
        listaPuestos.Add(new Puesto() { NombrePuesto = "Jefe de Sistemas"});
        listaPuestos.Add(new Puesto() { NombrePuesto = "Analista de Sistemas" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Arquitecto de Software" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Desarrollador" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Test QA" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Analista de Ventas" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Jefe de Ventas" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Jefe de Contabilidad" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Contador Senior" });
        listaPuestos.Add(new Puesto() { NombrePuesto = "Contador Junior" });


        // Obtener los nombres de los puestos que quieres añadir
        var listaNombrePuestos = listaPuestos.Select(g => g.NombrePuesto).ToList();

        // Obtener los nombres de los puestos coincidentes
        var puestosExistentes = await _dbContext.Set<Puesto>()
            .Where(g => listaNombrePuestos.Contains(g.NombrePuesto))
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Select(g => g.NombrePuesto)
            .ToListAsync();

        // Filtrar los puestos que no están en la base de datos
        var puestosFaltantes = listaPuestos
            .Where(g => !puestosExistentes.Contains(g.NombrePuesto))
            .ToList();

        // Añadir los puestos de trabajo que no existen en la base de datos
        if (puestosFaltantes.Any())
        {
            await _dbContext.Set<Puesto>().AddRangeAsync(puestosFaltantes);
            await _dbContext.SaveChangesAsync();
        }
    }
}