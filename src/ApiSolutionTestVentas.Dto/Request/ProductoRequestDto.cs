using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ApiSolutionTestVentas.Dto.Validations;

namespace ApiSolutionTestVentas.Dto.Request;

public class ProductoRequestDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = default!;
    [StringLength(200)]
    public string? Descripcion { get; set; } = default!;
    [StringLength(800)]
    public string? DescripcionExtensa { get; set; } = default!;
    [Required]
    public decimal PrecioUnitario { get; set; } = 0;
    //public string? ImageUrl { get; set; } = default!;
    [FileSizeValidation(1)]
    [FileTypeValidation(FileTypeGroup.Image)]
    public IFormFile? Image { get; set; } = default!;
    [Required]
    public int Stock { get; set; } = 0;

    [Required]
    public int CategoriaProductoId { get; set; } = 0;
}