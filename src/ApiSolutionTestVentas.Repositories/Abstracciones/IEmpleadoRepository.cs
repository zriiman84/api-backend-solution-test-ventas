using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IEmpleadoRepository : IRepositoryBase<Empleado>
{
    Task<ICollection<Empleado>> GetAsync(PaginationDto paginacion);
    Task<ICollection<EmpleadoInfo>> GetAsync(string? nombre, PaginationDto paginacion);
    Task<ICollection<Empleado>> GetAsync<TKey>(Expression<Func<Empleado, bool>> predicate, Expression<Func<Empleado, TKey>> orderBy, PaginationDto paginacion);
    Task<Empleado?> GetByIdAsync(int id);
    Task<Empleado?> GetByNroDocumentoAsync(string nroDocumento);
    Task<Empleado?> GetByEmailsync(string email);
}