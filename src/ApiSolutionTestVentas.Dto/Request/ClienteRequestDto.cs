using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class ClienteRequestDto
{
    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = default!;
    [Required]
    [StringLength(150)]
    public string Apellidos { get; set; } = default!;
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = default!;
}