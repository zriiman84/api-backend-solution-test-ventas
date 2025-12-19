using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class PuestoRepository : RepositoryBase<Puesto>, IPuestoRepository
{
    
    public PuestoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<Puesto>> GetAsync(string? nombrePuesto)
    {
        return await context.Set<Puesto>()
            .Where(x => x.NombrePuesto.Contains(!string.IsNullOrEmpty(nombrePuesto) ? nombrePuesto: String.Empty))
            .AsNoTracking()
            .ToListAsync();
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