using System.Globalization;
using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Repositories.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class EmpleadoRepository : RepositoryBase<Empleado>,IEmpleadoRepository
{
    private readonly IHttpContextAccessor _httpContext;
    
    public EmpleadoRepository(ApplicationDbContext context, IHttpContextAccessor httpContext):base(context)
    {
        this._httpContext = httpContext;
    }
    
    //B?squeda general con paginaci?n
    public async Task<ICollection<Empleado>> GetAsync(PaginationDto paginacion)
    {
        var queryable = context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .AsNoTracking()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.OrderBy(p => p.Id).Paginacion(paginacion).ToListAsync();
    }
    
    //B?squeda por nombre con paginaci?n
    public async Task<ICollection<EmpleadoInfo>> GetAsync(string? nombre, PaginationDto paginacion)
    {
        //Eager Loading
        /*return await context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .Where(x => x.Nombre.Contains(!string.IsNullOrEmpty(nombre) ?  nombre : String.Empty))
            .AsNoTracking()
            .ToListAsync();
            */
        
        //Lazy  Loading optimizado - EmpleadoInfo
        var queryable = context.Set<Empleado>()
            .Where(x => x.Nombre.Contains(!string.IsNullOrEmpty(nombre) ?  nombre : string.Empty))
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Select(x => new EmpleadoInfo
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellidos = x.Apellidos,
                Edad = x.Edad,
                NroDocumento = x.NroDocumento,
                Email = x.Email,
                Salario = x.Salario,
                JefeId = x.JefeId,
                PuestoId = x.PuestoId,
                Puesto = x.Puesto.NombrePuesto, 
                DepartamentoId = x.DepartamentoId,
                Departamento = x.Departamento.NombreDepartamento, 
                FechaIngreso = x.FechaIngreso.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                HoraIngreso = x.FechaIngreso.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                FechaBaja = x.FechaBaja != null 
                    ? x.FechaBaja.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) 
                    : String.Empty,
                HoraBaja = x.FechaBaja !=null 
                    ? x.FechaBaja.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) 
                    : String.Empty,
                Status = x.Status ? "Activo" : "Inactivo"
            }).AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.OrderBy(p => p.Id).Paginacion(paginacion).ToListAsync();
        
        //Raw query or SQL approach
        //var query = context.Database.SqlQueryRaw<EmpleadoInfo>("usp_ListarEmpleadosXNombre {0}", busqueda ?? string.Empty);
        //return await query.ToListAsync();
    }
    
    //B?squeda general sin paginaci?n
    public override async Task<ICollection<Empleado>> GetAsync()
    {
        return await context.Set<Empleado>()            
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<ICollection<Empleado>> GetAsync<TKey>(Expression<Func<Empleado, bool>> predicate, Expression<Func<Empleado, TKey>> orderBy, PaginationDto paginacion)
    {
        var queryable = context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .Where(predicate)
            .OrderBy(orderBy)
            .AsNoTracking()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.Paginacion(paginacion).ToListAsync();
    }
    
    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .FirstOrDefaultAsync(x => x.Id == id);
        //.FindAsync(id);
    }
    
    public async Task<Empleado?> GetByNroDocumentoAsync(string nroDocumento)
    {
        return await context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .Where(x => x.NroDocumento == nroDocumento)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Empleado?> GetByEmailsync(string email)
    {
        return await context.Set<Empleado>()
            .Include(x => x.Puesto) //Eager Loading
            .Include(x => x.Departamento) //Eager Loading
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
    }
    
    public override async Task<int> DeleteAsync(int id)
    {
        var item = await GetAsync(id);
        var result = 0;

        if (item is not null)
        {
            //Soft Delete
            item.Status = false;
            item.FechaBaja = DateTime.Now;
            result = await UpdateAsync();
        }
        else
        {
            result = -1;
        }
        
        return result;
    }
    
}