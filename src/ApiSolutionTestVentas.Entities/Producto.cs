namespace ApiSolutionTestVentas.Entities;

public class Producto : EntidadBase
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; } = default!;
    public string? DescripcionExtensa { get; set; } = default!;
    public decimal PrecioUnitario { get; set; } = 0; //precio actual
    public string? ImageUrl { get; set; } = default!;
    public int Stock { get; set; } = 0;
    public int CategoriaProductoId { get; set; } = 0;
    public CategoriaProducto CategoriaProducto { get; set; } = default!;
}