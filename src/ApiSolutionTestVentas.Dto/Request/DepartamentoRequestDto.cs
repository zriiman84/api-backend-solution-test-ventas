using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class DepartamentoRequestDto
{
    [Required]
    [StringLength(100)]
    public string NombreDepartamento { get; set; } = default!;
}