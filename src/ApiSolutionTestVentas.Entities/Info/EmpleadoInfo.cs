namespace ApiSolutionTestVentas.Entities.Info;

public class EmpleadoInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public int Edad { get; set; } = 0;
    public string NroDocumento { get; set; } = default!;
    public string Email { get; set; } = default!;
    public double Salario { get; set; } = 0;
    public int? JefeId { get; set; } = null;
    public int PuestoId { get; set; } = 0;
    public string Puesto { get; set; } = default!;
    public int DepartamentoId { get; set; } = 0;
    public string Departamento { get; set; } = default!;
    public string FechaIngreso { get; set; } = default!;
    public string HoraIngreso { get; set; } = default!;
    public string? FechaBaja { get; set; } = default!;
    public string? HoraBaja { get; set; } = default!;
    public string Status { get; set; } = default!;
}