namespace ApiSolutionTestVentas.Entities;

public class EntidadBase
{
    public int Id { get; set; }
    public bool Status { get; set; } = true; //soft delete
}