using System.Linq.Expressions;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Implementaciones;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntidadBase
{
    //Para admitir distintos DBcontext y no solo atarlo a un tipo de implementación
    protected readonly DbContext context;

    protected RepositoryBase(DbContext context)
    {
        this.context = context;
    }

    public virtual async Task<ICollection<TEntity>> GetAsync()
    {
        return await context.Set<TEntity>()            
            .AsNoTracking()
            .ToListAsync();
    }
    public virtual async Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await context.Set<TEntity>()
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<ICollection<TEntity>> GetAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy)
    {
        return await context.Set<TEntity>()
            .Where(predicate)
            .OrderBy(orderBy)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<TEntity?> GetAsync(int id)
    {
        return await context.Set<TEntity>()
            .FindAsync(id);
        //.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    //Retornará el id de la entidad creada
    public async Task<int> AddAsync(TEntity entity)
    {
        await context.Set<TEntity>()
            .AddAsync(entity);
        await context.SaveChangesAsync();
        return entity.Id;

    }
    public async Task<int> UpdateAsync()
    {
        return await context.SaveChangesAsync();
    }
    
    public virtual async Task<int> DeleteAsync(int id)
    {
        var item = await GetAsync(id);
        var result = 0;

        if (item is not null)
        {
            //Soft Delete
            item.Status = false;
            //context.Set<TEntity>().Remove(item); //Hard Delete
            result = await UpdateAsync();
        }
        else
        {
            result = -1;
        }

        return result;
    }
}