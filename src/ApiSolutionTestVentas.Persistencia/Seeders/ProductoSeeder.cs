using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class ProductoSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public ProductoSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        // Definir los productos que deseas añadir
        var listProductos = new List<Producto>()
        {
            new Producto
            {
                Nombre = "producto_dummy_1", Descripcion = "producto dummy 1", DescripcionExtensa = "",
                PrecioUnitario = 250, Stock = 20, CategoriaProductoId = 5
            },
            new Producto
            {
                Nombre = "producto_dummy_2", Descripcion = "producto dummy 2", DescripcionExtensa = "",
                PrecioUnitario = 350, Stock = 30, CategoriaProductoId = 5
            },
            new Producto
            {
                Nombre = "producto_dummy_3", Descripcion = "producto dummy 3", DescripcionExtensa = "",
                PrecioUnitario = 450, Stock = 40, CategoriaProductoId = 5
            },
            new Producto
            {
                Nombre = "producto_dummy_4", Descripcion = "producto dummy 4", DescripcionExtensa = "",
                PrecioUnitario = 550, Stock = 50, CategoriaProductoId = 5
            },
        };

        // Obtener los nombres de los productos que quieres añadir
        var listaNombreProductos = listProductos.Select(g => g.Nombre).ToList();

        // Obtener los nombres de los productos existentes en la base de datos
        var productosExistentes = await _dbContext.Set<Producto>()
            .Where(g => listaNombreProductos.Contains(g.Nombre))
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Select(g => g.Nombre)
            .ToListAsync();

        // Filtrar los productos que no están en la base de datos
        var productosFaltantes = listProductos
            .Where(g => !productosExistentes.Contains(g.Nombre))
            .ToList();

        // Añadir los géneros que no existen en la base de datos
        if (productosFaltantes.Any())
        {
            await _dbContext.Set<Producto>().AddRangeAsync(productosFaltantes);
            await _dbContext.SaveChangesAsync();
        }
    }
}