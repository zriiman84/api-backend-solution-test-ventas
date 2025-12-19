namespace ApiSolutionTestVentas.Entities;

public class CategoriaProducto : EntidadBase
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; } = default!;
}