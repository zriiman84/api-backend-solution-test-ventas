using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IVentaRepository : IRepositoryBase<Venta>
{
    // Búsqueda de ventas personalizada con paginación
    Task<ICollection<Venta>> GetAsync<TKey>(Expression<Func<Venta, bool>> predicate, Expression<Func<Venta, TKey>> orderBy, PaginationDto pagination);
    
    // Búsqueda e ventas personalizada
    Task<ICollection<Venta>> GetAsync(PaginationDto pagination);
    Task CrearTransaccionAsync(); 
    Task RollbackTransaccionAsync();
    Task<Venta?> GetVentaByIdAsync(int idVenta);
    Task<ICollection<VentaProducto>> GetDetalleVentaByIdVentaAsync(int idVenta); 
    
    Task<ICollection<VentaProducto>> GetDetalleVentaByListIdVentaAsync(ICollection<int> listaIdsVentas); 
    Task<VentaProducto?> GetItemVentaByIdAsync(int idItemVenta);
    Task<int> AddCabeceraVentaAsync(Venta venta); //Retorna el Id de la Venta creada
    Task<int> AddItemVentaAsync(VentaProducto itemVentaProducto);   //Retorna el Id del Item Venta Producto creado
    Task<int> AddDetalleVentaAsync(ICollection<VentaProducto> listaDetalleVenta); //Retorna la cantidad de items creados.
    Task FinalizarTransaccionAsync(); //Confirmar la transaccion en la BD

    //Reportes para Minimal API
    Task<ICollection<VentaReporteClienteInfo>> GetVentaReporteCliente(DateTime? fechaInicio, DateTime? fechaFin);
    Task<ICollection<VentaReporteProductoInfo>> GetVentaReporteProducto(DateTime? fechaInicio, DateTime? fechaFin);
}