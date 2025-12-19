using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiSolutionTestVentas.Repositories.Util;

public static class HttpContextExtensions
{
    //Método de extensión: Al objeto HttpContext se le extenderá el método InsertarHeaderHttpContext()
    //Este método agregará en el Header del Response el total de registros en la variable "TotalRegistros".
    public async static Task InsertarHeaderHttpContext<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        List<T> totalRecords = await queryable.ToListAsync();
        httpContext.Response.Headers.Add("TotalRegistros", totalRecords.Count.ToString()); //TotalRegistros --> será configurado así en CORS en Program.cs
    }
}