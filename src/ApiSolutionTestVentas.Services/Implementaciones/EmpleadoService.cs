using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IPuestoRepository _puestoRepository;
    private readonly IDepartamentoRepository _departamentoRepository;
    private readonly ILogger<EmpleadoService> _logger;
    private readonly IMapper _mapper;

    public EmpleadoService
    (
        IEmpleadoRepository empleadoRepository,
        IPuestoRepository puestoRepository,
        IDepartamentoRepository departamentoRepository,
        ILogger<EmpleadoService> logger,
        IMapper mapper)
    {
        this._empleadoRepository = empleadoRepository;
        this._puestoRepository = puestoRepository;
        this._departamentoRepository = departamentoRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<BaseResponseGeneric<EmpleadoResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<EmpleadoResponseDto>();

        try
        {
            var empleadoDb = await _empleadoRepository.GetByIdAsync(id);

            if (empleadoDb is null)
            {
                response.Message = $"No se encontró el empleado con id {id}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<EmpleadoResponseDto>(empleadoDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el empleado con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el empleado con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<EmpleadoResponseDto>> GetByNroDocumentoAsync(string nroDocumento)
    {
        var response = new BaseResponseGeneric<EmpleadoResponseDto>();

        try
        {
            var empleadoDb = await _empleadoRepository.GetByNroDocumentoAsync(nroDocumento);

            if (empleadoDb is null)
            {
                response.Message = $"No se encontró el empleado con Nro. Documento {nroDocumento}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<EmpleadoResponseDto>(empleadoDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el empleado con Nro. Documento  {nroDocumento}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el empleado con Nro. Documento  {nroDocumento}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<EmpleadoResponseDto>> GetByEmailAsync(string email)
    {
        var response = new BaseResponseGeneric<EmpleadoResponseDto>();

        try
        {
            var empleadoDb = await _empleadoRepository.GetByEmailsync(email);

            if (empleadoDb is null)
            {
                response.Message = $"No se encontró el empleado con email {email}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<EmpleadoResponseDto>(empleadoDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el empleado con email  {email}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el empleado con email  {email}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<EmpleadoResponseDto>>> GetAsync(string? nombre,
        PaginationDto paginacion)
    {
        var response = new BaseResponseGeneric<ICollection<EmpleadoResponseDto>>();

        try
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var listaEmpleadosInfDb = await _empleadoRepository.GetAsync(nombre, paginacion);
                response.Data = _mapper.Map<ICollection<EmpleadoResponseDto>>(listaEmpleadosInfDb);
            }
            else
            {
                var listaEmpleadosDb = await _empleadoRepository.GetAsync(paginacion);
                response.Data = _mapper.Map<ICollection<EmpleadoResponseDto>>(listaEmpleadosDb);
            }

            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de empleados con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de empleados.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }


    public async Task<BaseResponseGeneric<ICollection<EmpleadoResponseDto>>> GetAsync(string? nombres,
        string? apellidos, PaginationDto paginacion)
    {
        var response = new BaseResponseGeneric<ICollection<EmpleadoResponseDto>>();

        try
        {
            // 1. Definir el Predicado (filtro WHERE): Empleados 
            Expression<Func<Empleado, bool>> filtro = u =>
                u.Nombre.Contains(!string.IsNullOrEmpty(nombres) ? nombres : string.Empty)
                && u.Apellidos.Contains(!string.IsNullOrEmpty(apellidos) ? apellidos : string.Empty);

            // 2. Definir el Ordenamiento (ORDER BY): Ordenar por id
            //    (Aquí TKey es de tipo int)
            Expression<Func<Empleado, int>> orden = u => u.Id;

            // 3. Llamada al método asíncrono
            var listaEmpleadosDb = await _empleadoRepository.GetAsync(
                predicate: filtro,
                orderBy: orden,
                paginacion);

            response.Data = _mapper.Map<ICollection<EmpleadoResponseDto>>(listaEmpleadosDb);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de empleados con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de empleados.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(EmpleadoRequestDto empleadoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var puestoDb = await _puestoRepository.GetAsync(empleadoRequestDto.PuestoId);
            var departamentoDb = await _departamentoRepository.GetAsync(empleadoRequestDto.DepartamentoId);

            if (puestoDb is null || departamentoDb is null)
            {
                response.Message = $"El puesto de trabajo con id {empleadoRequestDto.PuestoId} " +
                                   $"y/o el departamento con id {empleadoRequestDto.DepartamentoId} " +
                                   "no existen.";
                _logger.LogWarning(response.Message);
                return response;
            }

            var empleado = _mapper.Map<Empleado>(empleadoRequestDto);
            //empleado.FechaIngreso = DateTime.Now; //No es necesario porque lo estoy manejando en el Configuration

            var idEmpleado = await _empleadoRepository.AddAsync(empleado);

            if (idEmpleado == 0) throw new Exception("Ocurrió un error al registrar el empleado en la BD.");

            response.Data = idEmpleado;
            response.Success = true;
            response.Message = $"Se registró correctamente el nuevo empleado con id {idEmpleado}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar un nuevo empleado";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> UpdateAsync(int id, EmpleadoRequestDto empleadoRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var empleadoDB = await _empleadoRepository.GetAsync(id);
            var puestoDb = await _puestoRepository.GetAsync(empleadoRequestDto.PuestoId);
            var departamentoDb = await _departamentoRepository.GetAsync(empleadoRequestDto.DepartamentoId);

            if (empleadoDB is null)
            {
                response.Message = $"El empleado con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            if (puestoDb is null || departamentoDb is null)
            {
                response.Message = $"El puesto de trabajo con id {empleadoRequestDto.PuestoId} " +
                                   $"y/o el departamento con id {empleadoRequestDto.DepartamentoId} " +
                                   "no existen.";
                _logger.LogWarning(response.Message);
                return response;
            }

            _mapper.Map(empleadoRequestDto, empleadoDB);
            var result = await _empleadoRepository.UpdateAsync();

            if (result == 0) throw new Exception($"Ocurrió un error en la BD al actualizar el empleado con id {id}..");
            
            response.Data = id;
            response.Success = true;
            response.Message = $"Se actualizó correctamente el empleado con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al actualizar el empleado con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            var rpta = await _empleadoRepository.DeleteAsync(id);
            
            if (rpta ==0)   throw new Exception($"Ocurrió un error en la BD al eliminar el empleado con id {id}..");

            if (rpta > 0)
            {
                response.Success = true;
                response.Message = $"Se eliminó correctamente el empleado con id {id}";
                _logger.LogInformation(response.Message);
            }
            else 
            {
                response.Message = $"El empleado con id {id} no fue encontrado.";
                _logger.LogWarning(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al eliminar el empleado con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}