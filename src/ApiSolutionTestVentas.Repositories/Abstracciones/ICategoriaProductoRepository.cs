using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface ICategoriaProductoRepository : IRepositoryBase<CategoriaProducto>
{
    Task<ICollection<CategoriaProducto>> GetAsync(string? nombreCategoria);
}