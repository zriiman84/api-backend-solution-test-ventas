using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class EmpleadoRequestDto 
{
    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = default!;
    [Required]
    [StringLength(150)]
    public string Apellidos { get; set; } = default!;
    [Required]
    public int Edad { get; set; } = 0;
    [Required]
    [StringLength(20)]
    public string NroDocumento { get; set; } = default!;
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = default!;
    [Required]
    public double Salario { get; set; } = 0;
    public int? JefeId { get; set; } = null;
    [Required]
    public int PuestoId { get; set; } = 0;
    [Required]
    public int DepartamentoId { get; set; } = 0;
    
}