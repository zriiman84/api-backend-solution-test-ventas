using Microsoft.Extensions.DependencyInjection;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class DataSeederInicial
{
    private readonly IServiceProvider _serviceProvider;

    public DataSeederInicial(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        // Obtener el contexto de la base de datos del servicio
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var categoriaSeeder = new CategoriaProductoSeeder(context);
            await categoriaSeeder.SeedAsync();

            var departamentoSeeder = new DepartamentoSeeder(context);
            await departamentoSeeder.SeedAsync();

            var puestoSeeder = new PuestoSeeder(context);
            await puestoSeeder.SeedAsync();

            var productoSeeder = new ProductoSeeder(context);
            await productoSeeder.SeedAsync();

        }
        
    }
}