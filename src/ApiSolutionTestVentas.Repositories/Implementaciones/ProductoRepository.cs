using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Repositories.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class ProductoRepository : RepositoryBase<Producto>, IProductoRepository
{
    private readonly IHttpContextAccessor _httpContext;
    
    public ProductoRepository(ApplicationDbContext context, IHttpContextAccessor httpContext) : base(context)
    {
        this._httpContext = httpContext;
    }

    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await context.Set<Producto>()
            .Include(p => p.CategoriaProducto)
            .Where(x => x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }


    public async Task<ICollection<Producto>> GetAsync(string? nombreProducto, PaginationDto paginacion)
    {
        var queryable =  context.Set<Producto>()
            .Include(p => p.CategoriaProducto)
            .Where(x => x.Nombre.Contains(!string.IsNullOrEmpty(nombreProducto) ? nombreProducto : string.Empty))
            .AsNoTracking()
            .AsQueryable();

        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.OrderBy(p => p.Id).Paginacion(paginacion).ToListAsync();
    }
    
    public async Task<ICollection<Producto>> GetListProductsByListIdAsync(ICollection<int> listaIds,  bool flagAsNoTracking)
    {
        if (flagAsNoTracking)
        {
            return await context.Set<Producto>()
                .Include(p => p.CategoriaProducto)
                .Where(p => listaIds.Contains(p.Id)) // Se traduce a WHERE Id IN (1, 5, 8...)
                .AsNoTracking()
                .ToListAsync();
        }
        else
        {
            return await context.Set<Producto>()
                .Where(p => listaIds.Contains(p.Id)) // Se traduce a WHERE Id IN (1, 5, 8...)
                .ToListAsync();
        }
        
      
    }
}