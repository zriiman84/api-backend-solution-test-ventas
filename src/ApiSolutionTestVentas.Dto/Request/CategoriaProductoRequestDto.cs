using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class CategoriaProductoRequestDto
{
    [Required]
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; } = default!;
}