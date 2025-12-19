using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class CategoriaProductoRepository : RepositoryBase<CategoriaProducto>, ICategoriaProductoRepository
{
    public CategoriaProductoRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<ICollection<CategoriaProducto>> GetAsync(string? nombreCategoria)
    {
        return await context.Set<CategoriaProducto>()
            .Where(x => x.Nombre.Contains(!string.IsNullOrEmpty(nombreCategoria) ? nombreCategoria : string.Empty))
            .AsNoTracking()
            .ToListAsync();
    }
}