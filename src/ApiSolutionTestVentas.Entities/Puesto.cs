namespace ApiSolutionTestVentas.Entities;

public class Puesto : EntidadBase
{
    public string NombrePuesto { get; set; } = default!;
    public string? DescripcionPuesto { get; set; } = default!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaBaja { get; set; }
}