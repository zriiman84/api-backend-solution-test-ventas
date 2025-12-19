namespace ApiSolutionTestVentas.Entities;

public class VentaProducto :  EntidadBase
{
    public int ProductoId { get; set; }
    public int VentaId { get; set; }
    public decimal SubTotal { get; set; } = 0;
    public int Cantidad { get; set; } = 0;
    public decimal PrecioCompra { get; set; } = 0; //Es el precio con el que se compró el producto.
    
    //navigation properties
    public Producto Producto { get; set; } = default!;
    public Venta Venta { get; set; } = default!;
}