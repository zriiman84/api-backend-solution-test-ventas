using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ApiSolutionTestVentas.Dto.Validations;

public class FileSizeValidation : ValidationAttribute
{
    private readonly int _maxSizeInMegabytes;

    public FileSizeValidation(int maxSizeInMegabytes)
    {
        _maxSizeInMegabytes = maxSizeInMegabytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        //Casteo incluso objetos nulos.
        //Si envío una palabra o un objeto no compatible con IformFile, formfile será NULL
        IFormFile? formfile = value as IFormFile; 

        if (formfile is null)
            //return ValidationResult.Success;
            return new ValidationResult("No es un tipo de archivo válido.");
        
        if (formfile.Length > _maxSizeInMegabytes * 1024 * 1024)
            return new ValidationResult($"El tamaño del archivo no debería exceder de {_maxSizeInMegabytes} Mb.");

        return ValidationResult.Success;
    }
}