using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IProductoRepository : IRepositoryBase<Producto>
{
    
    Task<Producto?> GetByIdAsync(int id);
    Task<ICollection<Producto>> GetAsync(string? nombreProducto, PaginationDto paginacion);
    Task<ICollection<Producto>> GetListProductsByListIdAsync(ICollection<int> listaIds, bool flagAsNoTracking );
}