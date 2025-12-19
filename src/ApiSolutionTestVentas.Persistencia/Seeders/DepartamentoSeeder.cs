using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class DepartamentoSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public DepartamentoSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        // Definir los departamentos que deseas añadir
        var listaDepartamentos = new List<Departamento>();
        listaDepartamentos.Add(new Departamento() { NombreDepartamento = "Tecnología y Sistemas" });
        listaDepartamentos.Add(new Departamento() { NombreDepartamento = "Ventas" });
        listaDepartamentos.Add(new Departamento() { NombreDepartamento = "Legal" });
        listaDepartamentos.Add(new Departamento() { NombreDepartamento = "Finanzas y Contabilidad" });
        listaDepartamentos.Add(new Departamento() { NombreDepartamento = "Recursos Humanos y Gestión del Talento" });


        // Obtener los nombres de los departamentos que quieres añadir
        var listaNombreDepartamentos = listaDepartamentos.Select(g => g.NombreDepartamento).ToList();

        // Obtener los nombres de las categorías coincidentes
        var departamentosExistentes = await _dbContext.Set<Departamento>()
            .Where(g => listaNombreDepartamentos.Contains(g.NombreDepartamento))
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Select(g => g.NombreDepartamento)
            .ToListAsync();

        // Filtrar los departamentos que no están en la base de datos
        var departamentosFaltantes = listaDepartamentos
            .Where(g => !departamentosExistentes.Contains(g.NombreDepartamento))
            .ToList();

        // Añadir los departamentos que no existen en la base de datos
        if (departamentosFaltantes.Any())
        {
            await _dbContext.Set<Departamento>().AddRangeAsync(departamentosFaltantes);
            await _dbContext.SaveChangesAsync();
        }
    }
}