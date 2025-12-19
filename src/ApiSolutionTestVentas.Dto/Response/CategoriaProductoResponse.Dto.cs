namespace ApiSolutionTestVentas.Dto.Response;

public class CategoriaProductoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; } = default!;
    public string Status { get; set; } = default!;
}