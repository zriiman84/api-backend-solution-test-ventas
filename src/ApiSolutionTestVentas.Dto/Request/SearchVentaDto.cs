namespace ApiSolutionTestVentas.Dto.Request;

public class SearchVentaDto
{
    public string? FechaInicio { get; set; } = default!;
    public string? FechaFin { get; set; } = default!;
}