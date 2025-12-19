namespace ApiSolutionTestVentas.Entities;

public class Venta : EntidadBase
{
    public int ClienteId { get; set; }
    public int? EmpleadoId { get; set; } = null;
    public DateTime FechaHoraVenta { get; set; }
    public string NumeroOperacion { get; set; } = default!;
    public decimal MontoTotalVenta { get; set; } = 0;
    public int CantidadTotalArticulos { get; set; } = 0;

    //navigation properties
    public virtual Cliente Cliente { get; set; } = default!;
    public virtual Empleado? Empleado { get; set; } = default!;
}