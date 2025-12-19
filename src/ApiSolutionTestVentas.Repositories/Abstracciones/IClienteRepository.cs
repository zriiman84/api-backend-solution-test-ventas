using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IClienteRepository : IRepositoryBase<Cliente>
{
    Task<Cliente?> GetAsync(string email);
    Task<ICollection<Cliente>> GetAsync(PaginationDto pagination);
    
    Task<ICollection<Cliente>> GetAsync<TKey>(Expression<Func<Cliente, bool>> predicate, Expression<Func<Cliente, TKey>> orderBy, PaginationDto paginacion);
}