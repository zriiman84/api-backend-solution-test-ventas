namespace ApiSolutionTestVentas.Dto.Response;

public class PuestoResponseDto
{
    public int Id { get; set; }
    public string NombrePuesto { get; set; } = default!;
    public string? DescripcionPuesto { get; set; } = default!;
    public string FechaHoraCreacion { get; set; } = default!;
    public string? FechaHoraBaja { get; set; } = default!;
    public string Status { get; set; } = default!;
}