using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class PuestoRequestDto
{
    [Required]
    [StringLength(100)]
    public string NombrePuesto { get; set; } = default!;
    [StringLength(200)]
    public string? DescripcionPuesto { get; set; } = default!;
}