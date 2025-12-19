using ApiSolutionTestVentas.Entities;

namespace ApiSolutionTestVentas.Dto.Request;

public class VentaRequestDto
{
    public int? EmpleadoId { get; set; } = null;
    public List<DetalleVenta>? DetalleVenta { get; set; } = default!;
}