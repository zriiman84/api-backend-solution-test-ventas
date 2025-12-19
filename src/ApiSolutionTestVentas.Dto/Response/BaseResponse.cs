namespace ApiSolutionTestVentas.Dto.Response;

public class BaseResponse
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; } = null;
    public string? ErrorMessage { get; set; } = null;
}