namespace ApiSolutionTestVentas.Entities;

public class Departamento : EntidadBase
{
    public string NombreDepartamento { get; set; } = default!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaBaja { get; set; }
}