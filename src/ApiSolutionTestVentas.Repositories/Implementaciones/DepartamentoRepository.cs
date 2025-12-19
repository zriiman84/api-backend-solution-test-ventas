using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public class DepartamentoRepository : RepositoryBase<Departamento>, IDepartamentoRepository
{
    public DepartamentoRepository(ApplicationDbContext context) : base(context)
    {
      
    }
   
    public async Task<ICollection<Departamento>> GetAsync(string? nombreDepartamento)
    {
        return await context.Set<Departamento>()
            .Where(x => x.NombreDepartamento.Contains(!string.IsNullOrEmpty(nombreDepartamento) ? nombreDepartamento : String.Empty))
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