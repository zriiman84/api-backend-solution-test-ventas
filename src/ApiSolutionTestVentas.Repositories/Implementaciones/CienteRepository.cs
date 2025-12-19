using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Repositories.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class ClienteRepository : RepositoryBase<Cliente>,IClienteRepository
{
    private readonly IHttpContextAccessor _httpContext;
    
    public ClienteRepository(ApplicationDbContext context, IHttpContextAccessor httpContext) : base(context)
    {
        this._httpContext = httpContext;
    }
    
    public async Task<Cliente?> GetAsync(string email)
    {
        return await context.Set<Cliente>()
            .Where(x => x.Email == email)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    
    //Listar clientes con paginación
    public async Task<ICollection<Cliente>> GetAsync(PaginationDto paginacion)
    {
        var queryable = context.Set<Cliente>()
            .AsNoTracking()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.OrderBy(p => p.Id).Paginacion(paginacion).ToListAsync();
    }
   
    //Listar búsqueda personalizada de clientes con paginación
    public async Task<ICollection<Cliente>> GetAsync<TKey>(Expression<Func<Cliente, bool>> predicate, Expression<Func<Cliente, TKey>> orderBy, PaginationDto paginacion)
    {
        var queryable = context.Set<Cliente>()
            .Where(predicate)
            .OrderBy(orderBy)
            .AsNoTracking()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.Paginacion(paginacion).ToListAsync();
    }
}