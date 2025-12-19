using System.Data;
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

public class VentaRepository : RepositoryBase<Venta>, IVentaRepository
{
    IHttpContextAccessor _httpContext;
    
    public VentaRepository(ApplicationDbContext context, IHttpContextAccessor httpContext) : base(context)
    {
        this._httpContext = httpContext;
    }

    //Búsqueda personalizada de la venta con paginación.
    public async Task<ICollection<Venta>> GetAsync<TKey>(Expression<Func<Venta, bool>> predicate,
        Expression<Func<Venta, TKey>> orderBy, PaginationDto pagination)
    {
        var queryable = context.Set<Venta>()
            .Include(x => x.Cliente) //Eager Loading
            .Include(x => x.Empleado) //Eager Loading
            .Where(predicate)
            .OrderBy(orderBy)
            .AsNoTracking()
            .IgnoreQueryFilters()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.Paginacion(pagination).ToListAsync();
    }
    
    //Búsqueda personalizada de la venta con paginación.
    public async Task<ICollection<Venta>> GetAsync (PaginationDto pagination)
    {
        var queryable = context.Set<Venta>()
            .Include(x => x.Cliente) //Eager Loading
            .Include(x => x.Empleado) //Eager Loading
            .AsNoTracking()
            .IgnoreQueryFilters()
            .AsQueryable();
        
        await _httpContext.HttpContext.InsertarHeaderHttpContext(queryable);
        return await queryable.Paginacion(pagination).ToListAsync();
    }

    public async Task CrearTransaccionAsync()
    {
        await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
    }

    public async Task RollbackTransaccionAsync()
    {
        await context.Database.RollbackTransactionAsync();
    }
    
    public async Task FinalizarTransaccionAsync()
    {
        await context.Database.CommitTransactionAsync();
    }
    
    public async Task<Venta?> GetVentaByIdAsync(int idVenta)
    {
        return await context.Set<Venta>()
            .Include(x => x.Cliente) //Eager Loading
            .Include(x => x.Empleado) //Eager Loading
            .Where(x => x.Id == idVenta)
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync();
    }
    
    public async Task<ICollection<VentaProducto>> GetDetalleVentaByIdVentaAsync(int idVenta)
    {
        return await context.Set<VentaProducto>()
            .Include(x => x.Producto) //Eager Loading
            .ThenInclude(x => x.CategoriaProducto) //Eager Loading para obtener la categoria del producto
            .Where(x => x.VentaId == idVenta)
            .AsNoTracking()
            .IgnoreQueryFilters() //Retornará los productos incluso inactivos.
            .ToListAsync(); 
    }

    public async Task<ICollection<VentaProducto>> GetDetalleVentaByListIdVentaAsync(ICollection<int> listaIdsVentas)
    {
        return await context.Set<VentaProducto>()
            .Include(x => x.Producto)
            .ThenInclude(x => x.CategoriaProducto) //Eager Loading para obtener la categoria del producto
            .Where(x => listaIdsVentas.Contains(x.VentaId)) // Se traduce a WHERE IdProducto IN (1, 5, 8...)
            .AsNoTracking()
            .IgnoreQueryFilters()
            .ToListAsync();
    }
    
    public async Task<VentaProducto?> GetItemVentaByIdAsync(int idItemVenta)
    {
        return await context.Set<VentaProducto>()
            .Include(x => x.Producto) //Eager Loading
            .ThenInclude(x => x.CategoriaProducto) //Eager Loading para obtener la categoria del producto
            .Where(x => x.Id == idItemVenta)
            .AsNoTracking()
            .IgnoreQueryFilters() //Retornará los productos incluso inactivos.
            .FirstOrDefaultAsync();
    }
    
    //Método para insertar la cabecera de la venta y retorna el Id de la Venta.
    public async Task<int> AddCabeceraVentaAsync(Venta venta)
    {
        venta.FechaHoraVenta = DateTime.Now;
        var operacion = await context.Set<Venta>().CountAsync() + 1;
        venta.NumeroOperacion = $"{operacion: 0000000}";
        await context.Set<Venta>().AddAsync(venta); //context.AddAsync(venta)
        await context.SaveChangesAsync(); 
        return venta.Id;
    }
    
    //Método para insertar un elemento detalle y retorna el id del item
    public async Task<int> AddItemVentaAsync(VentaProducto itemVentaProducto)
    {
        await context.Set<VentaProducto>().AddAsync(itemVentaProducto); //context.AddAsync(itemVentaProducto)
        await context.SaveChangesAsync(); 
        return itemVentaProducto.Id;
    }
    
    //Método más óptimo para insertar una lista de elementos de manera directa. Retorna la cantidad de elementos insertados.
    public async Task<int> AddDetalleVentaAsync(ICollection<VentaProducto> listaDetalleVenta)
    {
        await context.Set<VentaProducto>().AddRangeAsync(listaDetalleVenta);
        return await context.SaveChangesAsync(); 
    }

    public async Task<ICollection<VentaReporteClienteInfo>> GetVentaReporteCliente(DateTime? fechaInicio, DateTime? fechaFin)
    {
        IQueryable<VentaReporteClienteInfo> resultado;

        //Raw query or SQL approach
        var query = @"SELECT TOP 30
                        c.Email as EmailCliente, 
                        c.Nombre + ' ' + c.Apellidos as NombreCliente,
                        SUM(v.CantidadTotalArticulos) as CantidadTotalArticulos,
                        SUM(v.MontoTotalVenta) as MontoTotalVenta
                        FROM Ventas.Venta v 
                        INNER JOIN Ventas.Cliente c ON (v.ClienteId = c.Id)";

        if(fechaInicio !=null && fechaFin !=null)
        {
            query = query + @" WHERE v.FechaHoraVenta >= {0} AND v.FechaHoraVenta < {1} GROUP BY c.Email,c.Nombre + ' ' + c.Apellidos ORDER BY MontoTotalVenta DESC";
            resultado = context.Database.SqlQueryRaw<VentaReporteClienteInfo>(query, fechaInicio, fechaFin);

        }else if (fechaInicio != null && fechaFin is null)
        {
            query = query + @" WHERE v.FechaHoraVenta >= {0} GROUP BY c.Email,c.Nombre + ' ' + c.Apellidos ORDER BY MontoTotalVenta DESC";
            resultado = context.Database.SqlQueryRaw<VentaReporteClienteInfo>(query, fechaInicio);

        }
        else if (fechaInicio is null && fechaFin != null)
        {
            query = query + @" WHERE v.FechaHoraVenta < {0} GROUP BY c.Email,c.Nombre + ' ' + c.Apellidos ORDER BY MontoTotalVenta DESC";
            resultado = context.Database.SqlQueryRaw<VentaReporteClienteInfo>(query, fechaFin);
        }
        else
        {
            query = query + @"  GROUP BY c.Email,c.Nombre + ' ' + c.Apellidos ORDER BY MontoTotalVenta DESC";
            resultado = context.Database.SqlQueryRaw<VentaReporteClienteInfo>(query);
        }

            return await resultado.ToListAsync(); //Ejecutar la consulta en la BD
    }

    public async Task<ICollection<VentaReporteProductoInfo>> GetVentaReporteProducto(DateTime? fechaInicio, DateTime? fechaFin)
    {
        IQueryable<VentaReporteProductoInfo> resultado;

        //Raw query or SQL approach
        var query = @"SELECT TOP 30 
                        p.Id as IdProducto,
                        p.Nombre as NombreProducto,
                        SUM(vp.Cantidad) as CantidadTotalArticulos,
                        SUM(p.PrecioUnitario * vp.Cantidad) as MontoTotalProducto
                        FROM Ventas.Venta v 
                        INNER JOIN Ventas.VentaProducto vp ON (vp.VentaId = v.Id) 
                        INNER JOIN Ventas.Producto p ON (p.Id = vp.ProductoId)";

        if (fechaInicio != null && fechaFin != null)
        {
            query = query + @" WHERE v.FechaHoraVenta >= {0} AND v.FechaHoraVenta < {1} GROUP BY p.Id,p.Nombre ORDER BY MontoTotalProducto DESC, p.Id ASC";
            resultado = context.Database.SqlQueryRaw<VentaReporteProductoInfo>(query, fechaInicio, fechaFin);

        }
        else if (fechaInicio != null && fechaFin is null)
        {
            query = query + @" WHERE v.FechaHoraVenta >= {0} GROUP BY p.Id,p.Nombre ORDER BY MontoTotalProducto DESC, p.Id ASC";
            resultado = context.Database.SqlQueryRaw<VentaReporteProductoInfo>(query, fechaInicio);

        }
        else if (fechaInicio is null && fechaFin != null)
        {
            query = query + @" WHERE v.FechaHoraVenta < {0} GROUP BY p.Id,p.Nombre ORDER BY MontoTotalProducto DESC, p.Id ASC";
            resultado = context.Database.SqlQueryRaw<VentaReporteProductoInfo>(query, fechaFin);
        }
        else
        {
            query = query + @" GROUP BY p.Id,p.Nombre ORDER BY MontoTotalProducto DESC, p.Id ASC";
            resultado = context.Database.SqlQueryRaw<VentaReporteProductoInfo>(query);
        }

         return await resultado.ToListAsync(); //Ejecutar la consulta en la BD
    }

}