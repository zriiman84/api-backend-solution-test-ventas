namespace ApiSolutionTestVentas.Dto.Response;

public class DepartamentoResponseDto
{
    public int Id { get; set; }
    public string NombreDepartamento{ get; set; } = default!;
    public string FechaHoraCreacion { get; set; } = default!;
    public string? FechaHoraBaja { get; set; } = default!;
    public string Status { get; set; } = default!;
}