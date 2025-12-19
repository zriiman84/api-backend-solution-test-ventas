namespace ApiSolutionTestVentas.Dto.Response;

public class ProductoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; } = default!;
    public string? DescripcionExtensa { get; set; } = default!;
    public decimal PrecioUnitario { get; set; } = 0;
    public string? ImageUrl { get; set; } = default!;
    public int Stock { get; set; } = 0;

    public int CategoriaProductoId { get; set; } = default!;

    public string NombreCategoriaProducto { get; set; } = default!;
    public string Status { get; set; }  = default!;
}