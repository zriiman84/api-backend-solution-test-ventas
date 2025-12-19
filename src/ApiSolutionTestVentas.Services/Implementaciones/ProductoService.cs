using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ILogger<ProductoService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly IMapper _mapper;

    private readonly IFileStorage _fileStorage;
    private string _container;

    public ProductoService(IProductoRepository repository, ILogger<ProductoService> logger, IMapper mapper,
        IFileStorage fileStorage, IOptions<AppSettings> options)
    {
        this._productoRepository = repository;
        this._logger = logger;
        this._mapper = mapper;
        _options = options;
        this._fileStorage = fileStorage;
        _container = _options.Value.ConnectionStrings.ContainerNameBlob;
    }

    public async Task<BaseResponseGeneric<ProductoResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<ProductoResponseDto>();

        try
        {
            var productoDb = await _productoRepository.GetByIdAsync(id);

            if (productoDb is null)
            {
                response.Message = $"No se encontró el producto con id {id}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<ProductoResponseDto>(productoDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el producto con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el producto con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ProductoResponseDto>>> GetAsync(string? nombre,
        PaginationDto paginacion)
    {
        var response = new BaseResponseGeneric<ICollection<ProductoResponseDto>>();

        try
        {
            var listaProductos = await _productoRepository.GetAsync(nombre, paginacion);

            response.Data = _mapper.Map<ICollection<ProductoResponseDto>>(listaProductos);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de productos con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de productos.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(ProductoRequestDto productoRequest)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var producto = _mapper.Map<Producto>(productoRequest);

            //1. Registro el producto en la base de datos
            var idProducto = await _productoRepository.AddAsync(producto);

            if (idProducto == 0) throw new Exception("Ocurrió un error al registrar el producto en la BD.");

            //2. Obtengo el producto registrado
            producto = await _productoRepository.GetAsync(idProducto);

            if (producto is null) throw new Exception("Ocurrió un error al registrar el producto en la BD.");

            //3. Lógica para procesar la imagen enviada del producto y setear la URL de dicha imagen
            if (productoRequest.Image is not null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productoRequest.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(productoRequest.Image.FileName);
                    producto.ImageUrl = await _fileStorage.SaveFile(_container, content, extension,
                        productoRequest.Image.ContentType);
                }
                //3.1 Actualizo únicamente la URL
                await _productoRepository.UpdateAsync();
            }

            response.Data = idProducto;
            response.Success = true;
            response.Message = $"Se registró correctamente el nuevo producto con id {idProducto}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar un nuevo producto.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> UpdateAsync(int id, ProductoRequestDto productoRequest)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var productoDb = await _productoRepository.GetAsync(id);

            if (productoDb is null)
            {
                response.Message = $"El producto con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            var oldFilePath = productoDb.ImageUrl ?? string.Empty;
            _mapper.Map(productoRequest, productoDb);

            //Lógica para procesar la imagen enviada del producto y setear la URL de dicha imagen
            if (productoRequest.Image is not null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productoRequest.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(productoRequest.Image.FileName);
                    productoDb.ImageUrl = await _fileStorage.EditFile(_container, content, extension,
                        productoRequest.Image.ContentType, oldFilePath);
                }
            }

            var result = await _productoRepository.UpdateAsync();

            if (result == 0) throw new Exception($"Ocurrió un error en la BD al actualizar el producto con id {id}..");
            
            response.Data = id;
            response.Success = true;
            response.Message = $"Se actualizó correctamente el producto con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al actualizar el producto con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();
        string message = string.Empty;

        try
        {
            var productoDb = await _productoRepository.GetAsync(id);

            if (productoDb is not null)
            {
                if (!(await _fileStorage.DeleteFile(productoDb.ImageUrl ?? string.Empty, _container)))
                {
                    message = " No se pudo eliminar la imagen del producto.";
                }
            }
            else
            {
                response.Message = $"El producto con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            var rpta = await _productoRepository.DeleteAsync(id);

            if (rpta > 0)
            {
                response.Success = true;
                response.Message = $"Se eliminó correctamente el producto con id {id}." + message;
                _logger.LogInformation(response.Message);
            }
            else
            {
                throw new Exception($"Ocurrió un error en la BD al eliminar el producto con id {id}..");
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al eliminar el producto con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

}