using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IDepartamentoRepository : IRepositoryBase<Departamento>
{
    Task<ICollection<Departamento>> GetAsync(string? nombreDepartamento);
}