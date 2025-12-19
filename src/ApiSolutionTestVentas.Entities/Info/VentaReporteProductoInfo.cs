namespace ApiSolutionTestVentas.Entities.Info;

public class VentaReporteProductoInfo
{
    public int IdProducto { get; set; } = 0;
    public string NombreProducto { get; set; } = string.Empty;
    public int CantidadTotalArticulos { get; set; } = 0;
    public decimal MontoTotalProducto { get; set; } = 0;
}