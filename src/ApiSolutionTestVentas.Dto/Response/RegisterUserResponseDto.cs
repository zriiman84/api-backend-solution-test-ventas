namespace ApiSolutionTestVentas.Dto.Response;

public class RegisterUserResponseDto : LoginResponseDto
{
    public string UserId { get; set; } = default!;
}