using System.ComponentModel.DataAnnotations;

namespace ApiSolutionTestVentas.Dto.Request;

public class RegisterUserRequestDto
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = default!;
    
    [Required]
    [StringLength(150)]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(150)]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]
    [StringLength(80)]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(20)]
    public string DocumentNumber { get; set; } = default!;

    [Required]
    public int DocumentType { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public string Password { get; set; } = default!;

    [Compare(nameof(Password), ErrorMessage = "¡Las contraseñas no coinciden!")]
    public string ConfirmPassword { get; set; } = default!;
}