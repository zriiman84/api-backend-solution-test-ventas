using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authorization;

namespace ApiSolutionTestVentas.Api.MinimalApi;


public static class ReporteVentasProductoEndPoints
{
    public static void MapReporteVentasProducto(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/Reports/VentasProducto")
            .WithDescription("EndPoint para Reporte TOP 30 de mayores Ventas por Producto")
            .WithTags("Reports");

        group.MapGet("/", [Authorize(Roles = Entities.Constants.RoleAdmin)] async (IVentaService _ventaService, string? fechaInicio, string? fechaFin) =>
        {
            var responseReporte = await _ventaService.GetAsyncSaleReportByProduct(fechaInicio, fechaFin);

            if (!String.IsNullOrEmpty(responseReporte.ErrorMessage))
            {
                return Results.Problem(
                    title: "Error al obtener el Reporte de Ventaspor Producto.",
                    detail: $"Ocurrió un error inesperado. {responseReporte.ErrorMessage}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return responseReporte.Success ? Results.Ok(responseReporte) : Results.BadRequest(responseReporte);
        });


    }
}