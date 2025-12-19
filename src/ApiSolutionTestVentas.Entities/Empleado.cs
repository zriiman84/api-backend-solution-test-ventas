namespace ApiSolutionTestVentas.Entities;

public class Empleado : EntidadBase
{
    public string Nombre { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public int Edad { get; set; } = 0;
    public string NroDocumento { get; set; } = default!;
    public string Email { get; set; } = default!;
    public double Salario { get; set; } = 0;
    public int? JefeId { get; set; } = null;
    public int PuestoId { get; set; } = 0;
    public int DepartamentoId { get; set; } = 0;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaBaja { get; set; } = default!;
    //Modificamos a virtual para el Lazy Loading
    public virtual Puesto Puesto { get; set; } = default!;
    //Modificamos a virtual para el Lazy Loading
    public virtual Departamento Departamento { get; set;} = default!;
   
}