namespace ApiSolutionTestVentas.Dto.Response;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public string ExpirationDateTime{ get; set; } = default!;
}