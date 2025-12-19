using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class CategoriaProductoService : ICategoriaProductoService
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly ILogger<CategoriaProductoService> _logger;
    private readonly IMapper _mapper;

    public CategoriaProductoService(ICategoriaProductoRepository categoriaRepository,
        ILogger<CategoriaProductoService> logger, IMapper mapper)
    {
        this._categoriaRepository = categoriaRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<BaseResponseGeneric<CategoriaProductoResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<CategoriaProductoResponseDto>();

        try
        {
            var categoriaDb = await _categoriaRepository.GetAsync(id);

            if (categoriaDb is null)
            {
                response.Message = $"No se encontró la categoría con id {id}...";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<CategoriaProductoResponseDto>(categoriaDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente la categoría con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener la categoría con id {id}...";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<CategoriaProductoResponseDto>>> GetAsync(string? nombreCategoria)
    {
        var response = new BaseResponseGeneric<ICollection<CategoriaProductoResponseDto>>();

        try
        {
            var categorias = await _categoriaRepository.GetAsync(nombreCategoria);

            response.Data = _mapper.Map<ICollection<CategoriaProductoResponseDto>>(categorias);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de categorías con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de categorías.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(CategoriaProductoRequestDto categoriaRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var idCategoriaProducto =
                await _categoriaRepository.AddAsync(_mapper.Map<CategoriaProducto>(categoriaRequestDto));

            if (idCategoriaProducto == 0) throw new Exception("Ocurrió un error al registrar la categoría en la BD.");
            
            response.Data = idCategoriaProducto;
            response.Success = true;
            response.Message =
                $"Se registró correctamente la nueva categoría de producto con id {idCategoriaProducto}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar una nueva categoría.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> UpdateAsync(int id, CategoriaProductoRequestDto categoriaRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var categoriaDb = await _categoriaRepository.GetAsync(id);

            if (categoriaDb is null)
            {
                response.Message = $"La categoría con id {id} no fue encontrada.";
                _logger.LogWarning(response.Message);
                return response;
            }

            _mapper.Map(categoriaRequestDto, categoriaDb);
            var result = await _categoriaRepository.UpdateAsync();

            if (result == 0) throw new Exception($"Ocurrió un error en la BD al actualizar la categoría con id {id}..");
            
            response.Data = id;
            response.Success = true;
            response.Message = $"Se actualizó correctamente la categoría de producto con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al actualizar la categoría con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            var rpta = await _categoriaRepository.DeleteAsync(id);
            
            if (rpta == 0) throw new Exception($"Ocurrió un error en la BD al actualizar la categoría con id {id}..");

            if (rpta > 0)
            {
                response.Success = true;
                response.Message = $"Se eliminó correctamente la categoría de producto con id {id}";
                _logger.LogInformation(response.Message);
            }
            else 
            {
                response.Message = $"La categoria con id {id} no fue encontrada";
                _logger.LogWarning(response.Message);
            }
          
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al eliminar la categoría con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}