namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IFileStorage
{
    Task<string> SaveFile(string containerName, byte[] content, string extension, string contentType);
    Task<string> EditFile(string containerName, byte[] content, string extension, string contentType, string oldPath);
    Task<bool> DeleteFile(string filePath, string containerName);
    //Task<byte[]> GetFile(string filePath, string containerName);
}