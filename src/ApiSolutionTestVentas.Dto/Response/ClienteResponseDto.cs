namespace ApiSolutionTestVentas.Dto.Response;

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Status { get; set; } = default!;
}