using ApiSolutionTestVentas.Dto.Request;

namespace ApiSolutionTestVentas.Repositories.Util;

public static class IQueryableExtensions
{
    //Método de extensión: A cada objeto de tipo IQueryable<T> se le extenderá un código adicional al select de SQL
    //Se le agregará un OFFSET ROWS (saltar esos elementos) y un FETCH NEXT ROWS (tomar solo estos elementos)
    public static IQueryable<T> Paginacion<T>(this IQueryable<T> queryable, PaginationDto paginacion)
    {
        return queryable
            .Skip((paginacion.Page - 1) * paginacion.PageSize)
            .Take(paginacion.PageSize); 
    }
}