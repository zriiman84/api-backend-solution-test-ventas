using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class FileStorageAzure : IFileStorage
{
    private readonly ILogger<FileStorageAzure> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly string _azureConnectionString;

    public FileStorageAzure(ILogger<FileStorageAzure> logger, IOptions<AppSettings> options)
    {
        _logger = logger;
        _options = options;
        _azureConnectionString = _options.Value.ConnectionStrings.AzureStorage;
    }

    public async Task<bool> DeleteFile(string filePath, string containerName)
    {
        bool result = false;
        try
        {
            if (string.IsNullOrEmpty(filePath)) return true;
            
            var client = new BlobContainerClient(_azureConnectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var blob = client.GetBlobClient(
                Path.GetFileName(filePath)); //Ejemplo: client.GetBlobClient("empleado_789.jpg");
            result = await blob.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return result;
    }

    public async Task<string> EditFile(string containerName, byte[] content, string extension, string contentType,
        string oldPath)
    {
        await DeleteFile(oldPath, containerName);
        return await SaveFile(containerName, content, extension, contentType);
    }

    public async Task<string> SaveFile(string containerName, byte[] content, string extension, string contentType)
    {
        string blobUri = string.Empty;
        try
        {
            //BlobContainerClient permite operaciones sobre contenedores en AZURE
            var client = new BlobContainerClient(_azureConnectionString, containerName);
            await client.CreateIfNotExistsAsync(); //Intenta crear un nuevo contenedor si no existe

            //Establece la política de acceso al contenedor, para que los blobs sean públicos
            client.SetAccessPolicy(PublicAccessType.Blob);

            var fileName = $"{Guid.NewGuid()}{extension}"; //(ej. b3f9a...-... .png).

            //Obtiene un BobClient que representa ese blob específico dentro del contenedor
            var blob = client.GetBlobClient(fileName);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = contentType; //(ej. image/png, application/pdf)
            blobUploadOptions.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(new BinaryData(content), blobUploadOptions);

            // Ejemplo: https://midatastore01.blob.core.windows.net/imagenes/empleado_789.jpg
            blobUri = blob.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return blobUri;
    }
}