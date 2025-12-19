using System.Linq.Expressions;
using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Repositories.Abstracciones;

public interface IRepositoryBase<TEntity> where TEntity : EntidadBase
{
    Task<ICollection<TEntity>> GetAsync();
    Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<ICollection<TEntity>> GetAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy);
    Task<TEntity?> GetAsync(int id);
    Task<int> AddAsync(TEntity entity); //Retornará el id del elemento creado
    Task<int> UpdateAsync();    //Retornará la cantidad de elementos actualizados
    Task<int> DeleteAsync(int id);   //Retornará la cantidad de elementos eliminados
}
