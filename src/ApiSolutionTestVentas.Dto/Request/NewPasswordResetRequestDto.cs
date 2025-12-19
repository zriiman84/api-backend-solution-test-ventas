using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class NewPasswordResetRequestDto
{
    [EmailAddress]
    [StringLength(60)]
    public string Email { get; set; } = default!;
    
    [Required]
    public string Token { get; set; } = default!;
    
    [Required]
    public string NewPassword { get; set; } = default!;
    
    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "¡Las contraseñas no coinciden!")]
    public string ConfirmNewPassword { get; set; } = default!;
}