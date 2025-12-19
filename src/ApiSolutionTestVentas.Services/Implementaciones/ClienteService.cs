using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<ClienteService> _logger;
    private readonly IMapper _mapper;

    public ClienteService(IClienteRepository clienteRepository, ILogger<ClienteService> logger, IMapper mapper)
    {
        this._clienteRepository = clienteRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<BaseResponseGeneric<ICollection<ClienteResponseDto>>> GetAsync(PaginationDto paginacion)
    {
        var response = new BaseResponseGeneric<ICollection<ClienteResponseDto>>();

        try
        {
            var listaClientesDb = await _clienteRepository.GetAsync(paginacion);
            response.Data = _mapper.Map<ICollection<ClienteResponseDto>>(listaClientesDb);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "La lista de clientes actualmente está vacía.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de clientes con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de clientes.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ClienteResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<ClienteResponseDto>();

        try
        {
            var clienteDb = await _clienteRepository.GetAsync(id);

            if (clienteDb is null)
            {
                response.Message = $"No se encontró el cliente con id {id}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<ClienteResponseDto>(clienteDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el cliente con id {id}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el cliente con id {id}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ClienteResponseDto>> GetAsync(string email)
    {
        var response = new BaseResponseGeneric<ClienteResponseDto>();

        try
        {
            var clientesDb = await _clienteRepository.GetAsync(email);

            if (clientesDb is null)
            {
                response.Message = $"No se encontró el cliente con la email solicitado: {email}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<ClienteResponseDto>(clientesDb);
            response.Success = true;
            response.Message = $"Se obtuvo correctamente el cliente con email {email}...";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el cliente por email {email}.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ClienteResponseDto>>> GetAsync(string? nombres, string? apellidos,
        PaginationDto pagination)
    {
        var response = new BaseResponseGeneric<ICollection<ClienteResponseDto>>();

        try
        {
            // 1. Definir el Predicado (filtro WHERE): Empleados 
            Expression<Func<Cliente, bool>> filtro = u =>
                u.Nombre.Contains(!string.IsNullOrEmpty(nombres) ? nombres : String.Empty)
                && u.Apellidos.Contains(!string.IsNullOrEmpty(apellidos) ? apellidos : String.Empty);

            // 2. Definir el Ordenamiento (ORDER BY): Ordenar por Id
            //    (Aquí TKey es de tipo DateTime)
            Expression<Func<Cliente, int>> orden = u => u.Id;

            // 3. Llamada al método asíncrono
            var listaClientesDb = await _clienteRepository.GetAsync(
                predicate: filtro,
                orderBy: orden,
                pagination);

            response.Data = _mapper.Map<ICollection<ClienteResponseDto>>(listaClientesDb);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de clientes con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de clientes.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    /*
     //La inserción de un nuevo cliente se dará por el registro de un nuevo usuario mediante su EndPoint
    public async Task<BaseResponseGeneric<int>> AddAsync(ClienteRequestDto clienteRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var idCliente = await _clienteRepository.AddAsync(_mapper.Map<Cliente>(clienteRequestDto));

            if(idCliente == 0)  throw new Exception("Ocurrió un error al registrar el cliente en la BD.");

            response.Data = idCliente;
            response.Success = true;
            response.Message = $"Se registró correctamente el nuevo cliente con id {idCliente}";
            _logger.LogInformation(response.Message);

        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al registrar un nuevo cliente.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
    */
}