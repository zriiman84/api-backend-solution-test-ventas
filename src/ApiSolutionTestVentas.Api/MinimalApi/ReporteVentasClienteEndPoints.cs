using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSolutionTestVentas.Api.MinimalApi;

public static class ReporteVentasClienteEndPoints
{
    public static void MapReporteVentasCliente(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/Reports/VentasCliente")
            .WithDescription("EndPoint para Reporte de TOP 30 de mayores Ventas por Cliente")
            .WithTags("Reports");

        group.MapGet("/", [Authorize(Roles = Entities.Constants.RoleAdmin)] async (IVentaService _ventaService, [FromQuery] string? fechaInicio, [FromQuery] string? fechaFin) =>
        {
            var responseReporte = await _ventaService.GetAsyncSaleReportByClient(fechaInicio, fechaFin);

            if (!string.IsNullOrEmpty(responseReporte.ErrorMessage))
            {
                return Results.Problem(
                    title: "Error al obtener el Reporte por Clientes.",
                    detail: $"Ocurrió un error inesperado: {responseReporte.ErrorMessage}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return responseReporte.Success ? Results.Ok(responseReporte) : Results.BadRequest(responseReporte);
        });


    }
}