using ApiSolutionTestVentas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class CategoriaProductoSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public CategoriaProductoSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        var listaCategoriaProductos = new List<CategoriaProducto>()
        {
            new CategoriaProducto { Nombre = "Laptops", Descripcion = "Laptops de todas las marcas" },
            new CategoriaProducto { Nombre = "Celulares", Descripcion = "Celulares de todas las marcas" },
            new CategoriaProducto { Nombre = "Monitores / TV", Descripcion = "Monitores / Tv" },
            new CategoriaProducto { Nombre = "Accesorios Electrónicos", Descripcion = "Accesorios como cables, mouses, memorias USB, adaptadores, coolers, etc" },
            new CategoriaProducto { Nombre = "Otros", Descripcion = "Varios como sillas ergonómicas, escritorios, etc." },
        };

        // Obtener los nombres de las categorías que quieres añadir
        var listaNombreCategorias = listaCategoriaProductos.Select(g => g.Nombre).ToList();

        // Obtener los nombres de las categorías coincidentes
        var categoriasExistentes = await _dbContext.Set<CategoriaProducto>()
            .Where(g => listaNombreCategorias.Contains(g.Nombre))
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Select(g => g.Nombre)
            .ToListAsync();

        // Filtrar las categorías que no están en la base de datos
        var categoriasFaltantes = listaCategoriaProductos
            .Where(g => !categoriasExistentes.Contains(g.Nombre))
            .ToList();

        // Añadir las categorías que no existen en la base de datos
        if (categoriasFaltantes.Any())
        {
            await _dbContext.Set<CategoriaProducto>().AddRangeAsync(categoriasFaltantes);
            await _dbContext.SaveChangesAsync();
        }
    }
}