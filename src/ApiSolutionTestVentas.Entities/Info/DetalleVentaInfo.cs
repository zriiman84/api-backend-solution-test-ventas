namespace ApiSolutionTestVentas.Entities.Info;

public class DetalleVentaInfo
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = default!;
    public int CategoriaProductoId { get; set; } = default!;
    public string NombreCategoriaProducto { get; set; } = default!;
    public decimal Precio { get; set; } = 0; //Es la foto con el precio con el que se compró el producto.
    public int Cantidad { get; set; } = 0;
    public decimal SubTotal { get; set; } = 0;
   
}