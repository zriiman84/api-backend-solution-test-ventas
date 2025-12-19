namespace ApiSolutionTestVentas.Entities.Info;

public class VentaReporteClienteInfo
{
    public string EmailCliente { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;

    public int CantidadTotalArticulos { get; set; } = 0;
    public decimal MontoTotalVenta { get; set; } = 0;
}
