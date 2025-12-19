using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class DepartamentoService : IDepartamentoService
{
    private readonly IDepartamentoRepository _departamentoRepository;
    private readonly ILogger<DepartamentoService> _logger;
    private readonly IMapper _mapper;

    public DepartamentoService(IDepartamentoRepository departamentoRepository, ILogger<DepartamentoService> logger,
        IMapper mapper)
    {
        this._departamentoRepository = departamentoRepository;
        this._logger = logger;
        this._mapper = mapper;
    }


    public async Task<BaseResponseGeneric<DepartamentoResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<DepartamentoResponseDto>();

        try
        {
            var departamento = await _departamentoRepository.GetAsync(id);

            if (departamento is null)
            {
                response.Message = $"No se encontró el departamnto con id {id}...";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<DepartamentoResponseDto>(departamento);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el departamento con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el departamento con id {id}...";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<DepartamentoResponseDto>>> GetAsync(string? nombreDepartamento)
    {
        var response = new BaseResponseGeneric<ICollection<DepartamentoResponseDto>>();

        try
        {
            var listaDepartamentos = await _departamentoRepository.GetAsync(nombreDepartamento);
            response.Data = _mapper.Map<ICollection<DepartamentoResponseDto>>(listaDepartamentos);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de departamentos con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de departamentos.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(DepartamentoRequestDto departamentoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var idDepartamento =
                await _departamentoRepository.AddAsync(_mapper.Map<Departamento>(departamentoRequestDto));

            if (idDepartamento == 0) throw new Exception("Ocurrió un error al registrar el departamento en la BD.");

            response.Data = idDepartamento;
            response.Success = true;
            response.Message = $"Se registró correctamente el nuevo departamento con id {idDepartamento}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar un nuevo departamento.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> UpdateAsync(int id, DepartamentoRequestDto departamentoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var departamentoDb = await _departamentoRepository.GetAsync(id);

            if (departamentoDb is null)
            {
                response.Message = $"El departamento con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            _mapper.Map(departamentoRequestDto, departamentoDb);
            var result = await _departamentoRepository.UpdateAsync();

            if (result == 0)
                throw new Exception($"Ocurrió un error en la BD al actualizar el departamento con id {id}..");
            
            response.Data = id;
            response.Success = true;
            response.Message = $"Se actualizó correctamente el departamento con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al actualizar el departamento con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            var rpta = await _departamentoRepository.DeleteAsync(id);
            
            if(rpta == 0) throw new Exception($"Ocurrió un error en la BD al eliminar el departamento con id {id}..");

            if (rpta > 0)
            {
                response.Success = true;
                response.Message = $"Se eliminó correctamente el departamento con id {id}";
                _logger.LogInformation(response.Message);
            }
            else 
            {
                response.Message = $"El departamento con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
            }
          
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al eliminar el departamento con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}