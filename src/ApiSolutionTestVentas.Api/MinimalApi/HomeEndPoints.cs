using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Services.Abstracciones;

namespace ApiSolutionTestVentas.Api.MinimalApi;


public static class HomeEndPoints
{
    public static void MapHome(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Home", async (
            ICategoriaProductoService categoriaProductoService,
            IProductoService _productoService) =>
        {
            var paginacion = new PaginationDto() { Page = 1, PageSize = 100 };
            var responseCategorias = await categoriaProductoService.GetAsync(String.Empty);
            var responseProductos = await _productoService.GetAsync(String.Empty, paginacion);

            if (!String.IsNullOrEmpty(responseCategorias.ErrorMessage) && !String.IsNullOrEmpty(responseProductos.ErrorMessage))
            {
                string detalle = $"{responseCategorias.ErrorMessage} | {responseProductos.ErrorMessage}";
                return Results.Problem(
                    title: "Error al obtener las categorias y productos",
                    detail: $"Ocurrió un error inesperado. {detalle}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Ok(new
            {
                Categorias = responseCategorias.Data ?? new List<CategoriaProductoResponseDto>(),
                Productos = responseProductos.Data ?? new List<ProductoResponseDto>(),
                Success = true
            });


        }).WithDescription("Permite mostrar los endpoints de la pagina principal").WithTags("Home"); ;
    }
}