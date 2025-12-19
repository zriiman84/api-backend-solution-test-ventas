using ApiSolutionTestVentas.Entities.Info;

namespace ApiSolutionTestVentas.Dto.Response;

public class VentaResponseDto
{
    public int IdVenta { get; set; }
    public int ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public string FechaVenta { get; set; } = default!;
    public string HoraVenta { get; set; } = default!;
    public string NumeroOperacion { get; set; } = default!;
    public string NombreCompletoCliente { get; set; } = default!;
    public string? NombreCompletoEmpleado { get; set; }
    public decimal MontoTotalVenta { get; set; } = 0;
    public int CantidadTotalArticulos { get; set; } = 0;
    public List<DetalleVentaInfo> DetalleVenta { get; set; } = default!;

}