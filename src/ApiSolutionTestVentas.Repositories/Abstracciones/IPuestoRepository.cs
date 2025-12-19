using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IPuestoRepository : IRepositoryBase<Puesto>
{
    Task<ICollection<Puesto>> GetAsync(string? nombrePuesto);
}