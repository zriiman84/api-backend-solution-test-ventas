using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class PuestoService : IPuestoService
{
    private readonly IPuestoRepository _puestoRepository;
    private readonly ILogger<PuestoService> _logger;
    private readonly IMapper _mapper;

    public PuestoService(IPuestoRepository puestoRepository, ILogger<PuestoService> logger, IMapper mapper)
    {
        this._puestoRepository = puestoRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<BaseResponseGeneric<PuestoResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<PuestoResponseDto>();

        try
        {
            var puesto = await _puestoRepository.GetAsync(id);

            if (puesto is null)
            {
                response.Message = $"No se encontró el puesto de trabajo con id {id}...";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<PuestoResponseDto>(puesto);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el puesto de trabajo con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el puesto de trabajo con id {id}...";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<PuestoResponseDto>>> GetAsync(string? nombrePuesto)
    {
        var response = new BaseResponseGeneric<ICollection<PuestoResponseDto>>();

        try
        {
            var listPuestos = await _puestoRepository.GetAsync(nombrePuesto);
            response.Data = _mapper.Map<ICollection<PuestoResponseDto>>(listPuestos);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias con el campo de búsqueda.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se encontraron  coincidencias con el campo de búsqueda para {listPuestos.Count} elementos.";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de puestos de trabajo.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(PuestoRequestDto puestoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var idPuesto = await _puestoRepository.AddAsync(_mapper.Map<Puesto>(puestoRequestDto));

            if (idPuesto == 0) throw new Exception("Ocurrió un error al registrar el puesto de trabajo en la BD.");

            response.Data = idPuesto;
            response.Success = true;
            response.Message = $"Se registró correctamente el nuevo puesto de trabajo con id {idPuesto}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar un nuevo puesto de trabajo";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> UpdateAsync(int id, PuestoRequestDto puestoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var puestoDb = await _puestoRepository.GetAsync(id);

            if (puestoDb is null)
            {
                response.Message = $"El puesto de trabajo con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            _mapper.Map(puestoRequestDto, puestoDb);
            var result = await _puestoRepository.UpdateAsync();

            if (result == 0)
                throw new Exception($"Ocurrió un error en la BD al actualizar el puesto de trabajo con id {id}..");
            
            response.Data = id;
            response.Success = true;
            response.Message = $"Se actualizó correctamente el puesto de trabajo con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al actualizar el puesto de trabajo con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            var rpta = await _puestoRepository.DeleteAsync(id);
            
            if (rpta == 0)  throw new Exception($"Ocurrió un error en la BD al eliminar el puesto de trabajo con id {id}..");

            if (rpta > 0)
            {
                response.Success = true;
                response.Message = $"Se eliminó correctamente el puesto de trabajo con id {id}";
                _logger.LogInformation(response.Message);
            }
            else 
            {
                response.Message = $"El puesto de trabajo con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
            }
          
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al eliminar el puesto de trabajo con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}