using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class ChangePasswordRequestDto
{
    [Required]
    public string OldPassword { get; set; } = default!;
    
    [Required]
    public string NewPassword { get; set; } = default!;
}