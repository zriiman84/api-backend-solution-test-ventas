using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ApiSolutionTestVentas.Dto.Validations;

public class FileTypeValidation : ValidationAttribute
{
    private readonly string[]? _validTypes;

    public FileTypeValidation(string[] validTypes)
    {
        this._validTypes = validTypes;
    }

    public FileTypeValidation(FileTypeGroup fileTypeGroup)
    {
        switch (fileTypeGroup)
        {
            case FileTypeGroup.Image: _validTypes = ["image/jpeg", "image/png", "image/jpg"]; break;
            case  FileTypeGroup.Pdf:  _validTypes = ["application/pdf"]; break;
            case  FileTypeGroup.Word:  _validTypes = ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"]; break;
            case  FileTypeGroup.Excel:  _validTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]; break;
            case  FileTypeGroup.Txt:   _validTypes = ["text/plain"]; break;
            case  FileTypeGroup.Csv:   _validTypes = ["text/csv"]; break;
            case  FileTypeGroup.Json:   _validTypes = ["application/json"]; break;
            case  FileTypeGroup.Mp3:   _validTypes = ["audio/mpeg"]; break;
            case  FileTypeGroup.Mp4:   _validTypes = ["avideo/mp4"]; break;
        }
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (_validTypes is null || _validTypes.Length == 0)
            return new ValidationResult("Ningún tipo válido de archivo fue encontrado, por favor contacta al administrador.");

        //Casteo incluso objetos nulos.
        //Si envío una palabra o un objeto no compatible con IformFile, formfile será NULL
        IFormFile? formfile = value as IFormFile; 
        
        if (formfile is null)
            //return ValidationResult.Success;
            return new ValidationResult("No es un tipo de archivo válido.");

        if (!_validTypes.Contains(formfile.ContentType))
            return new ValidationResult($"Tipo de archivo no válido, debe ser uno de los siguientes: {string.Join(",", _validTypes)}");

        return ValidationResult.Success;
    }
}

public enum FileTypeGroup
{
    Image,
    Pdf,
    Word,
    Excel,
    Txt,
    Csv,
    Json,
    Mp4,
    Mp3
}